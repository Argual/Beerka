using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beerka.Web.Helpers;
using Beerka.Web.Models;
using Beerka.Web.Services;
using Beerka.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Beerka.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        BeerkaWebService _service;
        public ShoppingCartController(BeerkaWebService service)
        {
            _service = service;
        }

        // GET: ShoppingCartController
        public ActionResult Index()
        {

            ShoppingCart shoppingCart = SessionHelper.GetShoppingCart(HttpContext.Session, "ShoppingCart", _service);

            return View(shoppingCart);
        }

        // GET: ShoppingCartController/AddItem
        [HttpGet]
        public IActionResult AddItem(int id)
        {
            if (!_service.GetProducts().Exists(p => p.ID == id))
            {
                NotFound();
            }

            ViewData["ProductID"] = id;

            ShoppingCartItemViewModel item = new ShoppingCartItemViewModel
            {
                Amount = 1,
                PackagingTypeID = Array.IndexOf(Product.Packaging.PackagingTypes,Product.Packaging.Unit),
                Product = _service.GetProduct(id),
            };

            var cart = SessionHelper.GetShoppingCart(HttpContext.Session, "ShoppingCart", _service);
            if (cart.Items.Exists(i=>i.Product.ID==id))
            {
                var cartItem = cart.Items.Single(i => i.Product.ID == id);
                item.Amount = cartItem.PackAmount;
                item.PackagingTypeID = Array.IndexOf(Product.Packaging.PackagingTypes, cartItem.PackagingType);
            }

            return View(item);
        }

        // POST: ShoppingCartController/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(int id, ShoppingCartItemViewModel item)
        {
            if (item==null || !_service.GetProducts().Exists(p=>p.ID==id))
            {
                return RedirectToAction("Index","Home");
            }

            ViewData["ProductID"] = id;

            item.Product = _service.GetProduct(id);

            var shoppingCart = SessionHelper.GetShoppingCart(HttpContext.Session, "ShoppingCart", _service);

            switch (_service.ValidateShoppingCartItem(shoppingCart,_service.GetProduct(id),Product.Packaging.PackagingTypes[item.PackagingTypeID],item.Amount))
            {
                case ShoppingCartItemError.InvalidPackaging:
                    ModelState.AddModelError("PackagingType", "Specified packaging is unavailable for this product!");
                    break;
                case ShoppingCartItemError.InvalidAmount:
                    ModelState.AddModelError("Amount", "Specified amount is invalid for this product! It must be equal or less than the remaining stock!");
                    break;
                default:
                    break;
            }

            if (!ModelState.IsValid)
            {
                return View("AddItem", item);
            }

            var shoppingCartItem = new ShoppingCart.ShoppingCartItem
            {
                Product = item.Product,
                PackAmount = item.Amount,
                PackagingType = Product.Packaging.PackagingTypes[item.PackagingTypeID]
            };

            if (shoppingCart.Items.Exists(i=>i.Product.ID==item.Product.ID))
            {
                int index = shoppingCart.Items.FindIndex(i => i.Product.ID == item.Product.ID);
                shoppingCart.Items[index] = shoppingCartItem;
            }
            else
            {
                shoppingCart.Items.Add(shoppingCartItem);
            }

            if (shoppingCartItem.Amount<=0)
            {
                shoppingCart.Items.Remove(shoppingCartItem);
            }

            SessionHelper.SetShoppingCart(HttpContext.Session,"ShoppingCart",shoppingCart);

            return RedirectToAction("Index");
        }

        public IActionResult DeleteItem(int id)
        {
            var shoppingCart = SessionHelper.GetShoppingCart(HttpContext.Session, "ShoppingCart", _service);
            if (shoppingCart.Items.Select(i=>i.Product.ID).Contains(id))
            {
                shoppingCart.Items.Remove(shoppingCart.Items.Find(i => i.Product.ID == id));
                SessionHelper.SetShoppingCart(HttpContext.Session, "ShoppingCart", shoppingCart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Empty()
        {
            SessionHelper.SetShoppingCart(HttpContext.Session, "ShoppingCart", new ShoppingCart());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Finalize()
        {
            return View(new FinalizationViewModel());
        }

        [HttpPost]
        public IActionResult Finalize(FinalizationViewModel finalization)
        {
            if (finalization==null)
            {
                return View();
            }

            ShoppingCart cart = SessionHelper.GetShoppingCart(HttpContext.Session, "ShoppingCart", _service);

            if (cart.Items.Count<=0)
            {
                return RedirectToAction("Index");
            }

            Order order = new Order
            {
                CustomerName = finalization.Name,
                CustomerAddress = finalization.Address,
                CustomerEmail = finalization.Email,
                CustomerPhone = finalization.Phone,
                IsDelivered = false
            };

            List<Tuple<Product, int>> productOrders = new List<Tuple<Product, int>>();

            foreach (var item in cart.Items)
            {
                if (_service.GetProduct(item.Product.ID).Stock-_service.GetProductReservedAmount(item.Product) <=0)
                {
                    return RedirectToAction("Result", "Home", new { messageTitle = "Failed Order", messageSummary = "Order Failed!", messageDetails = "'"+ item.Product.Name+"' is out of stock." });
                }
                productOrders.Add(new Tuple<Product, int>(item.Product, item.Amount));
            }

            try
            {
                _service.AddOrder(order, productOrders);
            }catch(Exception)
            {
                return RedirectToAction("Result", "Home", new { messageTitle = "Failed Order", messageSummary = "Order Failed!", messageDetails = "Something went wrong while trying to register your order." });
            }

            SessionHelper.SetShoppingCart(HttpContext.Session, "ShoppingCart", new ShoppingCart());

            return RedirectToAction("Result","Home",new { messageTitle="Succesful Order", messageSummary="Order Registered!", messageDetails="Your order was registered succesfully." });
        }

        public IActionResult Result()
        {
            return View();
        }

    }
}
