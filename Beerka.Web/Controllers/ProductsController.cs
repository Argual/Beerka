using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Beerka.Web.Models;
using Beerka.Web.Services;
using Microsoft.AspNetCore.Http;
using Beerka.Persistence.Services;

namespace Beerka.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BeerkaWebService _service;

        public ProductsController(BeerkaWebService service)
        {
            _service = service;
        }

        // GET: Products
        public IActionResult Index(int? id, string sortOrderManufacturer="", bool changeSortOrderManufacturer = false, string sortOrderPrice="", bool changeSortOrderPrice = false, int page = 0)
        {

            if (id==null || !_service.GetSubCategories().Exists(c=>c.ID==id))
            {
                return NotFound();
            }

            var allProducts = _service.GetProducts().Where(p=>p.SubCategoryID==id && p.Stock-_service.GetProductReservedAmount(p)>0).ToList();

            string[] sortOrders = { "asc", "desc"};

            if (!sortOrders.Contains(sortOrderManufacturer))
            {
                sortOrderManufacturer = "asc";
            }
            if (changeSortOrderManufacturer)
            {
                sortOrderManufacturer = sortOrderManufacturer switch
                {
                    ("asc") => "desc",
                    _ => "asc",
                };
                ViewData["SortOrderManufacturer"] = sortOrderManufacturer;
                allProducts = sortOrderManufacturer switch
                {
                    ("asc") => allProducts.OrderBy(p => p.Manufacturer).ToList(),
                    _ => allProducts.OrderByDescending(p => p.Manufacturer).ToList(),
                };
            }
            

            if (!sortOrders.Contains(sortOrderPrice))
            {
                sortOrderPrice = "asc";
            }
            if (changeSortOrderPrice)
            {
                sortOrderPrice = sortOrderPrice switch
                {
                    ("asc") => "desc",
                    _ => "asc",
                };
                ViewData["SortOrderPrice"] = sortOrderPrice;
                allProducts = sortOrderPrice switch
                {
                    ("asc") => allProducts.OrderBy(p => p.PriceNet).ToList(),
                    _ => allProducts.OrderByDescending(p => p.PriceNet).ToList(),
                };
            }
            

            int pageMaxItems = 20;

            if (page * pageMaxItems > allProducts.Count)
            {
                NotFound();
            }

            if (page<0)
            {
                page = 0;
            }

            ViewData["PageNumber"] = page;
            int pageCount = allProducts.Count / pageMaxItems;
            if (allProducts.Count % pageMaxItems > 0)
            {
                pageCount++;
            }
            ViewData["PageCount"] = pageCount;

            int itemsToGet = allProducts.Count-(page)*pageMaxItems;
            if (itemsToGet>pageMaxItems)
            {
                itemsToGet = pageMaxItems;
            }
            var products = allProducts.GetRange(page * pageMaxItems, itemsToGet);

            for (int i = 0; i < products.Count; i++)
            {
                products[i].Stock -= _service.GetProductReservedAmount(products[i]);
            }

            return View(products);
        }
    }
}
