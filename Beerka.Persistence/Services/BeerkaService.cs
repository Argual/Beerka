using Beerka.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Beerka.Persistence.Services
{
    public class BeerkaService
    {
        protected readonly BeerkaContext _context;

        public BeerkaService(BeerkaContext context)
        {
            _context = context;
        }

        #region MainCategory related services

        /// <summary>
        /// Adds a new main category with given name to the list of main categories.
        /// </summary>
        /// <param name="name">The name of the new category.</param>
        public void AddMainCategory(string name)
        {
            AddMainCategory(new MainCategory() { Name = name });
        }

        /// <summary>
        /// Adds given main category to the list of main categories.
        /// </summary>
        /// <param name="mainCategory">Main category to add.</param>
        public void AddMainCategory(MainCategory mainCategory)
        {
            if (mainCategory==null)
            {
                throw new ArgumentNullException("Main category to add can not be null!",nameof(mainCategory));
            }
            _context.MainCategories.Add(mainCategory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes the given main category from the list of main categories.
        /// </summary>
        /// <param name="mainCategory">Main category to remove.</param>
        public void RemoveMainCategory(MainCategory mainCategory)
        {
            _context.MainCategories.Remove(mainCategory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes the main category with given ID from the list of main categories.
        /// </summary>
        /// <param name="id">The ID of the main category to be removed.</param>
        public void RemoveMainCategory(int id)
        {
            var mainCategoryToRemove = _context.MainCategories.FirstOrDefault(c=>c.ID==id);
            RemoveMainCategory(mainCategoryToRemove);
        }

        /// <summary>
        /// Removes the main category with given name from the list of main categories.
        /// </summary>
        /// <param name="name">The name of the main category to be removed.</param>
        public void RemoveMainCategory(string name)
        {
            var mainCategoryToRemove = _context.MainCategories.FirstOrDefault(c => c.Name == name);
            RemoveMainCategory(mainCategoryToRemove);
        }

        /// <summary>
        /// Gets the list of main categories.
        /// </summary>
        /// <returns>List of main categories.</returns>
        public List<MainCategory> GetMainCategories()
        {
            return _context.MainCategories.Include(mc => mc.SubCategories).ToList();
        }

        /// <summary>
        /// Gets the main category belonging to the given ID.
        /// </summary>
        /// <param name="id">ID of the main category to get.</param>
        /// <returns>Main category belonging to the given ID.</returns>
        public MainCategory GetMainCategory(int id)
        {
            return _context.MainCategories.Include(mc => mc.SubCategories).Single(mc => mc.ID == id);
        }

        /// <summary>
        /// Gets the main category with given name.
        /// </summary>
        /// <param name="name">Name of the main category to get.</param>
        /// <returns>Main category with given name.</returns>
        public MainCategory GetMainCategory(string name)
        {
            return _context.MainCategories.Include(mc => mc.SubCategories).Single(mc => mc.Name == name);
        }

        #endregion

        #region SubCategory related services

        /// <summary>
        /// Adds a new subcategory with given name and main category to the list of subcategories.
        /// </summary>
        /// <param name="name">Name of the new subcategory.</param>
        /// <param name="mainCategoryName">Name of the main category this subcategory belongs to.</param>
        public void AddSubCategory(string name, string mainCategoryName)
        {
            SubCategory subCategory = new SubCategory()
            {
                Name = name,
                MainCategoryID = GetMainCategory(mainCategoryName).ID
            };
            AddSubCategory(subCategory);
        }

        /// <summary>
        /// Adds given subcategory to the list of subcategories.
        /// </summary>
        /// <param name="subCategory">Subcategory to add.</param>
        public void AddSubCategory(SubCategory subCategory)
        {
            if (subCategory==null)
            {
                throw new ArgumentNullException("Subcategory to add can not be null!", nameof(subCategory));
            }
            _context.SubCategories.Add(subCategory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes the given subcategory from the list of subcategories.
        /// </summary>
        /// <param name="subCategory">Subcategory to be removed.</param>
        public void RemoveSubCategory(SubCategory subCategory)
        {
            _context.SubCategories.Remove(subCategory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes subcategory belonging to the given ID from the list of subcategories.
        /// </summary>
        /// <param name="id">ID belonging to the subcategory to be removed.</param>
        public void RemoveSubCategory(int id)
        {
            var subCategoryToRemove = _context.SubCategories.FirstOrDefault(sc => sc.ID == id);
            RemoveSubCategory(subCategoryToRemove);
        }

        /// <summary>
        /// Removes the subcategory indicated by its and its main category's name.
        /// </summary>
        /// <param name="mainCategoryName">Name of the main category the subcategory to be removed belongs to.</param>
        /// <param name="subCategoryName">Name of the subcategory to be removed.</param>
        public void RemoveSubCategory(string subCategoryName, string mainCategoryName)
        {
            var subCategoryToRemove = _context.MainCategories.Include(mc=>mc.SubCategories)
                .FirstOrDefault(mc=>mc.Name==mainCategoryName)
                .SubCategories.FirstOrDefault(sc => sc.Name == subCategoryName);
            RemoveSubCategory(subCategoryToRemove);
        }

        /// <summary>
        /// Gets the list of subcategories.
        /// </summary>
        /// <returns>List of subcategories.</returns>
        public List<SubCategory> GetSubCategories()
        {
            return _context.SubCategories.Include(sc => sc.MainCategory).ToList();
        }

        /// <summary>
        /// Gets the subcategory belonging to the given ID.
        /// </summary>
        /// <param name="id">ID of the subcategory to get.</param>
        /// <returns>Subcategory belonging to the given ID.</returns>
        public SubCategory GetSubCategory(int id)
        {
            return _context.SubCategories.Include(sc => sc.MainCategory).Single(sc => sc.ID == id);
        }

        /// <summary>
        /// Gets the subcategory indicated by its and its main category's name.
        /// </summary>
        /// <param name="mainCategoryName">Name of the main category the subcategory to get belongs to.</param>
        /// <param name="subCategoryName">Name of the subcategory to get.</param>
        /// <returns>Subcategory indicated by its and its main category's name.</returns>
        public SubCategory GetSubCategory(string subCategoryName, string mainCategoryName)
        {
            return GetMainCategory(mainCategoryName).SubCategories.Single(sc => sc.Name == subCategoryName);
        }

        #endregion

        #region Product related services

        public int GetProductReservedAmount(int productID)
        {
            return _context.ProductOrders.Where(po => po.ProductID == productID).Sum(po => po.Amount);
        }

        public int GetProductReservedAmount(Product product)
        {
            if (product==null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return GetProductReservedAmount(product.ID);
        }

        /// <summary>
        /// Adds a product to the list of products.
        /// </summary>
        /// <param name="product">Product to add.</param>
        public Product AddProduct(Product product)
        {
            if (product==null)
            {
                throw new ArgumentNullException("Product to add can not be null!",nameof(product));
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        /// <summary>
        /// Adds a new product to the list.
        /// </summary>
        /// <param name="name">Name of the product.</param>
        /// <param name="manufacturer">Manufacturer of the product.</param>
        /// <param name="modelNumber">Model number of the product.</param>
        /// <param name="description">Description of the product.</param>
        /// <param name="subCategoryID">ID of the subcategory this product belongs to.</param>
        /// <param name="stock">Amount of product in stock.</param>
        /// <param name="priceNet">Net price (without tax, etc) of the product.</param>
        /// <param name="packagingType">Packaging of the product.</param>
        public Product AddProduct(string name, string manufacturer, string modelNumber, string description, int subCategoryID, int stock, int priceNet, Product.Packaging.PackagingType packagingType)
        {
            var product = new Product
            {
                Name = name,
                Manufacturer = manufacturer,
                ModelNumber = modelNumber,
                Description = description,
                SubCategoryID = subCategoryID,
                PriceNet = priceNet,
                Stock = stock,
                PackagingType = packagingType
            };
            product = AddProduct(product);
            return product;
        }

        /// <summary>
        /// Updates the matching product in the context with the given product's data.
        /// </summary>
        public bool UpdateProduct(Product updatedProduct)
        {
            try
            {
                var product = _context.Products.Single(p => p.ID == updatedProduct.ID);
                product.Name = updatedProduct.Name;
                product.Manufacturer = updatedProduct.Manufacturer;
                product.PriceNet = updatedProduct.PriceNet;
                product.Stock = updatedProduct.Stock;
                product.Description = updatedProduct.Description;
                product.SubCategoryID = updatedProduct.SubCategoryID;
                product.ModelNumber = updatedProduct.ModelNumber;
                product.PackagingType = updatedProduct.PackagingType;
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the product with given ID.
        /// </summary>
        /// <param name="id">ID of the product to get.</param>
        /// <returns>Product with given ID.</returns>
        public Product GetProduct(int id)
        {
            return _context.Products.Include(p => p.SubCategory).ThenInclude(sc=>sc.MainCategory).Single(predicate => predicate.ID == id);
        }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <returns>List of products.</returns>
        public List<Product> GetProducts()
        {
            return _context.Products.Include(p => p.SubCategory).ThenInclude(sc => sc.MainCategory).ToList();
        }

        /// <summary>
        /// Removes given product from the list of products.
        /// </summary>
        /// <param name="product">Product to remove.</param>
        public void RemoveProduct(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes product with given id from the list of products.
        /// </summary>
        /// <param name="id">ID of the product to be removed.</param>
        public void RemoveProduct(int id)
        {
            var productToRemove = _context.Products.Single(predicate => predicate.ID == id);
            RemoveProduct(productToRemove);
        }

        #endregion

        #region Order related services

        /// <summary>
        /// Adds an order to the list of orders.
        /// </summary>
        /// <param name="order">Order to add.</param>
        /// <param name="productOrders">"List of ordered products with the product and the ordered amount respectively."</param>
        public Order AddOrder(Order order, IEnumerable<Tuple<Product, int>> productOrders)
        {
            if (order==null)
            {
                throw new ArgumentNullException("Order to add can not be null!",nameof(order));
            }
            if (!order.IsDelivered)
            {
                foreach (var productOrder in productOrders)
                {
                    if (productOrder.Item2 > productOrder.Item1.Stock)
                    {
                        throw new ArgumentException("Order contains a request for 'ID:" + productOrder.Item1.ID + ", Name:" + productOrder.Item1.Name + "' which can not be fulfilled due to low stock!");
                    }
                }
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var productOrder in productOrders)
            {
                ProductOrder productOrderToAdd = new ProductOrder
                {
                    ProductID = productOrder.Item1.ID,
                    OrderID = order.ID,
                    Amount = productOrder.Item2
                };
                _context.ProductOrders.Add(productOrderToAdd);
            }

            _context.SaveChanges();
            return order;
        }

        /// <summary>
        /// Adds a new order to the list of orders.
        /// </summary>
        /// <param name="customerName">Name of the customer.</param>
        /// <param name="customerAddress">Address of the customer.</param>
        /// <param name="customerPhone">Phone number of the customer.</param>
        /// <param name="customerEmail">Email address of the customer.</param>
        /// <param name="isDelivered">Whether or not the ordered products have been delivered.</param>
        /// <param name="productOrders">Collection of product orders. First value of a product order is the ordered product, the second value is the amount ordered..</param>
        public Order AddOrder(string customerName, string customerAddress, string customerPhone, string customerEmail, bool isDelivered, IEnumerable<Tuple<Product,int>> productOrders)
        {
            var order = new Order
            {
                CustomerName = customerName,
                CustomerAddress = customerAddress,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                IsDelivered = isDelivered
            };

            AddOrder(order,productOrders);
            return order;
        }

        /// <summary>
        /// Gets the order with given ID.
        /// </summary>
        /// <param name="id">ID of the order to get.</param>
        /// <returns>Order with given ID.</returns>
        public Order GetOrder(int id)
        {
            return _context.Orders
                .Include(o => o.ProductOrders).ThenInclude(po => po.Order)
                .Include(o => o.ProductOrders).ThenInclude(po => po.Product)
                .ThenInclude(p => p.SubCategory).ThenInclude(sc=>sc.MainCategory)
                .ThenInclude(mc => mc.SubCategories)
                .Single(o => o.ID == id);
        }

        /// <summary>
        /// Updates the matching order's delivery status in the context with the given order's data.
        /// </summary>
        public bool UpdateOrderDeliveryStatus(Order updatedOrder)
        {
            try
            {
                Order oldOrder = GetOrder(updatedOrder.ID);

                if (oldOrder.IsDelivered==updatedOrder.IsDelivered)
                {
                    return true;
                }

                int sign = 1;
                if (!oldOrder.IsDelivered && updatedOrder.IsDelivered)
                {
                    sign = -1;
                }

                foreach (var oldProductOrder in oldOrder.ProductOrders)
                {
                    if (updatedOrder.ProductOrders.Any(po=>po.ProductID==oldProductOrder.ProductID && po.OrderID == oldProductOrder.OrderID))
                    {
                        var product = GetProduct(oldProductOrder.ProductID);
                        product.Stock += sign * oldProductOrder.Amount;
                    }
                }

                oldOrder.IsDelivered = updatedOrder.IsDelivered;
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the list of orders.
        /// </summary>
        /// <returns>List of orders.</returns>
        public List<Order> GetOrders()
        {
            return _context.Orders
                .Include(o => o.ProductOrders).ThenInclude(po => po.Order)
                .Include(o => o.ProductOrders).ThenInclude(po => po.Product)
                .ThenInclude(p => p.SubCategory).ThenInclude(sc => sc.MainCategory)
                .ThenInclude(mc=>mc.SubCategories)
                .ToList();
        }

        /// <summary>
        /// Removes given order from the list of orders.
        /// </summary>
        /// <param name="order">Order to remove.</param>
        public void RemoveOrder(Order order)
        {
            if (!order.IsDelivered)
            {
                // If an undelivered order is removed, the undelivered products can be added back to the stock.
                foreach (var productOrder in order.ProductOrders)
                {
                    productOrder.Product.Stock += productOrder.Amount;
                }
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes the order with given ID from the list of orders.
        /// </summary>
        /// <param name="id">ID of the order to be removed.</param>
        public void RemoveOrder(int id)
        {
            var orderToRemove = GetOrder(id);
            RemoveOrder(orderToRemove);
        }

        #endregion

    }
}
