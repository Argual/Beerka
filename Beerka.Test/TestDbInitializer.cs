using Beerka.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Beerka.Test
{
    public static class TestDbInitializer
    {
        public static void Initialize(BeerkaContext context)
        {
            List<MainCategory> defaultMainCategories = new List<MainCategory>()
            {
                new MainCategory
                {
                    Name="MC1",
                    ID=1
                },
                new MainCategory
                {
                    Name="MC2",
                    ID=2
                }
            };
            foreach (var mainCategory in defaultMainCategories)
            {
                context.MainCategories.Add(mainCategory);
            }

            List<SubCategory> defaultSubCategories = new List<SubCategory>()
            {
                new SubCategory
                {
                    Name = "MC1 SC1",
                    MainCategoryID = 1,
                    ID=1
                },
                new SubCategory
                {
                    Name = "MC1 SC2",
                    MainCategoryID = 1,
                    ID=2
                },
                new SubCategory
                {
                    Name = "MC2 SC3",
                    MainCategoryID = 2,
                    ID=3
                },
                new SubCategory
                {
                    Name = "MC2 SC4",
                    MainCategoryID = 2,
                    ID=4
                },
            };
            foreach (var subCategory in defaultSubCategories)
            {
                context.SubCategories.Add(subCategory);
            }

            List<Product> defaultProducts = new List<Product>
            {
                new Product
                {
                    ID=1,
                    SubCategoryID=1,
                    Name="MC1 SC1 P1",
                    Description="Desc P1",
                    Manufacturer="Manuf P1",
                    ModelNumber="ModelNum P1",
                    PriceNet=100,
                    Stock=1000,
                    PackagingType=Product.Packaging.Unit
                },
                new Product
                {
                    ID=2,
                    SubCategoryID=2,
                    Name="MC1 SC2 P2",
                    Description="Desc P2",
                    Manufacturer="Manuf P2",
                    ModelNumber="ModelNum P2",
                    PriceNet=200,
                    Stock=2000,
                    PackagingType=Product.Packaging.Crate
                },
                new Product
                {
                    ID=3,
                    SubCategoryID=3,
                    Name="MC2 SC3 P3",
                    Description="Desc P3",
                    Manufacturer="Manuf P3",
                    ModelNumber="ModelNum P3",
                    PriceNet=300,
                    Stock=3000,
                    PackagingType=Product.Packaging.Shrinkwrap
                },
                new Product
                {
                    ID=4,
                    SubCategoryID=4,
                    Name="MC2 SC4 P4",
                    Description="Desc P4",
                    Manufacturer="Manuf P4",
                    ModelNumber="ModelNum P4",
                    PriceNet=400,
                    Stock=4000,
                    PackagingType=Product.Packaging.Shrinkwrap
                },
                new Product
                {
                    ID=5,
                    SubCategoryID=1,
                    Name="MC1 SC1 P5",
                    Description="Desc P5",
                    Manufacturer="Manuf P5",
                    ModelNumber="ModelNum P5",
                    PriceNet=500,
                    Stock=5000,
                    PackagingType=Product.Packaging.Shrinkwrap
                }
            };
            foreach (var product in defaultProducts)
            {
                context.Products.Add(product);
            }

            List<Order> defaultOrders = new List<Order>()
            {
                new Order
                {
                    ID=1,
                    IsDelivered = false,
                    CustomerName = "Mr O1",
                    CustomerAddress = "Address O1",
                    CustomerEmail = "O1@mail.com",
                    CustomerPhone = "06301111111"
                },
                new Order
                {
                    ID=2,
                    IsDelivered = true,
                    CustomerName = "Mrs O2",
                    CustomerAddress = "Address O2",
                    CustomerEmail = "O2@mail.com",
                    CustomerPhone = "06302222222"
                }
            };
            foreach (var order in defaultOrders)
            {
                context.Orders.Add(order);
            }

            List<ProductOrder> defaultProductOrders = new List<ProductOrder>
            {
                new ProductOrder
                {
                    OrderID=1,
                    ProductID=1,
                    Amount=10
                },
                new ProductOrder
                {
                    OrderID=1,
                    ProductID=2,
                    Amount=10
                },
                new ProductOrder
                {
                    OrderID=2,
                    ProductID=3,
                    Amount=10
                }
            };
            foreach (var productOrder in defaultProductOrders)
            {
                context.ProductOrders.Add(productOrder);
            }

            context.SaveChanges();

        }
    }
}
