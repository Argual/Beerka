using Beerka.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beerka.Persistence
{
    public static class DBInitializer
    {
        static readonly Random random = new Random();

        /// <summary>
        /// Generates flavors for the given template and adds them to the list of products.
        /// </summary>
        /// <param name="maxCount">The max amount of products to add.</param>
        /// <param name="service">The service the context can be reached through.</param>
        /// <param name="template">The template that serves as base for the generated products.</param>
        /// <param name="flavorPool">Flavors that will be randomly selected to be appended to the product.</param>
        /// <param name="appendPool">Additional words that will be used to create additional variations for the product.</param>
        private static void AddFakeBeverages(int maxCount, BeerkaService service, Product template, IEnumerable<string> flavorPool, IEnumerable<string> appendPool=null)
        {
            if (appendPool==null)
            {
                appendPool = new List<string>();
            }

            List<string> unusedFlavors = new List<string>(flavorPool);

            int productCount = service.GetProducts().Where(p => p.SubCategoryID == template.SubCategoryID).Count();
            while (maxCount>0 && unusedFlavors.Count>0)
            {
                string flavor = unusedFlavors[random.Next(0,unusedFlavors.Count)];
                unusedFlavors.Remove(flavor);
                productCount++;

                for (int i = -1; i < appendPool.Count(); i++)
                {
                    string apndMN = "";
                    string apndN = "";
                    string apndD = "";

                    if (i>=0)
                    {
                        string poolElement = appendPool.ElementAt(i);

                        apndMN = ":" + i.ToString();
                        apndN = " " + poolElement;
                        apndD = " " + poolElement + ".";
                    }

                    Product fake = new Product
                    {
                        Name = template.Name + " " + flavor + apndN,
                        Description = template.Description + " " + flavor + "." + apndD,
                        Manufacturer = template.Manufacturer,
                        ModelNumber = template.ModelNumber + "/" + productCount.ToString().PadLeft(3,'0') + apndMN,
                        PriceNet = template.PriceNet * random.Next(80, 120) / 100,
                        Stock = template.Stock * random.Next(80, 120) / 100,
                        SubCategoryID = template.SubCategoryID,
                        PackagingType = Product.Packaging.PackagingTypes[random.Next(0, Product.Packaging.PackagingTypes.Length)]
                    };

                    maxCount--;
                    service.AddProduct(fake);
                }
            }
        }

        private static List<Tuple<Product,int>> GetRandomProductOrders(BeerkaContext context, int count = 3)
        {
            BeerkaService service = new BeerkaService(context);

            List<Tuple<Product, int>> productOrders = new List<Tuple<Product, int>>();
            var products = context.Products.Where(p=>p.Stock-service.GetProductReservedAmount(p)>0).ToList();
            int productCount = products.Count();

            if (productCount<count)
            {
                throw new ArgumentException("The number of product orders requested can not be greater than the amount of avaliable products the store offers.", nameof(count));
            }

            List<int> productNumbers = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int productCandidateNumber = random.Next() % productCount;
                while (productNumbers.Contains(productCandidateNumber))
                {
                    productCandidateNumber = (productCandidateNumber + 1) % productCount;
                }
                productNumbers.Add(productCandidateNumber);
            }

            foreach (var pn in productNumbers)
            {
                Tuple<Product, int> po = new Tuple<Product, int>(products[pn], random.Next(1, products[pn].Stock-service.GetProductReservedAmount(products[pn]) + 1));
                productOrders.Add(po);
            }

            return productOrders;
        }

        /// <summary>
        /// Initializes the database with example data, if there is none.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="clean">Whether or not to clear the database before initializing.</param>
        public static async Task InitializeAsync(IServiceProvider serviceProvider, bool clean = false)
        {
            BeerkaContext context = serviceProvider.GetRequiredService<BeerkaContext>();

            if (clean)
            {
                context.Database.EnsureDeleted();
            }

            context.Database.EnsureCreated();

            context.Database.Migrate();

            BeerkaService service = new BeerkaService(context);

            if (!context.Users.Any())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<Employee>>();
                var guest = new Employee
                {
                    Email = "guest@guestmail.com",
                    UserName = "guest",
                    Name = "Sterling Archer",
                };
                var test = new Employee
                {
                    Email = "test@testmail.com",
                    UserName = "test",
                    Name = "John Test",
                };
                await userManager.CreateAsync(guest, "guest");
                await userManager.CreateAsync(test, "test");
            }

            if (!context.MainCategories.Any())
            {
                // There aren't any main categories!

                service.AddMainCategory("Alcoholic beverages");
                service.AddMainCategory("Non-alcoholic beverages");
            }

            if (!context.SubCategories.Any())
            {
                // There are no subcategories!

                service.AddSubCategory("Beers","Alcoholic beverages");
                service.AddSubCategory("Wines", "Alcoholic beverages");

                service.AddSubCategory("Carbonated mineral waters", "Non-alcoholic beverages");
                service.AddSubCategory("Non-carbonated mineral waters", "Non-alcoholic beverages");

                service.AddSubCategory("Carbonated soft drinks", "Non-alcoholic beverages");
                service.AddSubCategory("Non-carbonated soft drinks", "Non-alcoholic beverages");

            }

            if (!context.Products.Any())
            {
                string[] fruits = {
                    "Apple",
                    "Apricot",
                    "Banana",
                    "Blueberry",
                    "Cherry",
                    "Fig",
                    "Grape",
                    "Grapefruit",
                    "Guava",
                    "Honeydew",
                    "Kiwifruit",
                    "Lemon",
                    "Lime",
                    "Mandarin",
                    "Mango",
                    "Nashi",
                    "Nectarine",
                    "Orange",
                    "Passionfruit",
                    "Peach",
                    "Pear",
                    "Persimmon",
                    "Pineapple",
                    "Plum",
                    "Pomegranate",
                    "Rambutan",
                    "Raspberry",
                    "Rockmelon",
                    "Strawberry",
                    "Watermelon"
                };
                string[] variantsSoftDrink = {
                    "Limited",
                    "Diet",
                    "Zero"
                };
                string[] variantsBeer = { 
                    "Brown",
                    "Pale"
                };

                Product duff = new Product
                {
                    Name = "Duff Beer",
                    Manufacturer = "Duff Breweries",
                    ModelNumber = "DB0001",
                    Description = "Simply wonderful. Some say you can never get enough of it.",
                    Stock = 1000,
                    PackagingType = Product.Packaging.Crate,
                    PriceNet = 250,
                    SubCategoryID = service.GetSubCategory("Beers", "Alcoholic beverages").ID
                };

                service.AddProduct(duff);
                AddFakeBeverages(random.Next(30,60), service, duff, fruits, variantsBeer);

                Product patriot = new Product
                {
                    Name = "Pawtucket Patriot Ale",
                    Manufacturer = "Pawtucket Brewery",
                    ModelNumber = "PPA2387",
                    Description = "Now with zero percent duff!",
                    Stock = 1200,
                    PackagingType = Product.Packaging.Crate,
                    PriceNet = 250,
                    SubCategoryID = service.GetSubCategory("Beers", "Alcoholic beverages").ID
                };

                service.AddProduct(patriot);
                AddFakeBeverages(random.Next(30, 60), service, patriot, fruits, variantsBeer);

                Product rockminer = new Product
                {
                    Name = "Rockminer",
                    Manufacturer = "Rockmine Brewery",
                    ModelNumber = "RM011",
                    Description = "For parties.",
                    Stock = 800,
                    PackagingType = Product.Packaging.Crate,
                    PriceNet = 200,
                    SubCategoryID = service.GetSubCategory("Beers", "Alcoholic beverages").ID
                };

                service.AddProduct(rockminer);
                AddFakeBeverages(random.Next(30, 60), service, rockminer, fruits, variantsBeer);

                service.AddProduct(new Product
                {
                    Name = "Dragons' Tears",
                    Manufacturer = "Dragonstone Nest",
                    ModelNumber = "GT-1112",
                    Description = "Some people just drink and know things.",
                    Stock = 675,
                    PackagingType = Product.Packaging.Unit,
                    PriceNet = 3000,
                    SubCategoryID = service.GetSubCategory("Wines", "Alcoholic beverages").ID
                });

                service.AddProduct(new Product
                {
                    Name = "Ozhpri Bloodwine",
                    Manufacturer = "Ozhpri's Winery",
                    ModelNumber = "9624_MA",
                    Description = "True wine for true warriors.",
                    Stock =96,
                    PackagingType = Product.Packaging.Tray,
                    PriceNet = 4096,
                    SubCategoryID = service.GetSubCategory("Wines", "Alcoholic beverages").ID
                });

                service.AddProduct(new Product
                {
                    Name = "Aqua",
                    Manufacturer = "Aqua Co.",
                    ModelNumber = "A-001C",
                    Description = "Quality carbonated mineral water.",
                    Stock = 9989,
                    PackagingType = Product.Packaging.Tray,
                    PriceNet = 75,
                    SubCategoryID = service.GetSubCategory("Carbonated mineral waters", "Non-alcoholic beverages").ID
                });

                service.AddProduct(new Product
                {
                    Name = "Aqua Calm",
                    Manufacturer = "Aqua Co.",
                    ModelNumber = "A-001NC",
                    Description = "Quality non-carbonated mineral water.",
                    Stock = 9723,
                    PackagingType = Product.Packaging.Tray,
                    PriceNet = 85,
                    SubCategoryID = service.GetSubCategory("Non-carbonated mineral waters", "Non-alcoholic beverages").ID
                });

                var nukaCola = new Product
                {
                    Name = "Nuka-Cola",
                    Manufacturer = "Nuka-Cola Corporation",
                    ModelNumber = "NCC-2044",
                    Description = "Made using the original Nuka-Cola recipe!",
                    Stock = 5475,
                    PackagingType = Product.Packaging.Shrinkwrap,
                    PriceNet = 330,
                    SubCategoryID = service.GetSubCategory("Carbonated soft drinks", "Non-alcoholic beverages").ID
                };

                service.AddProduct(nukaCola);
                AddFakeBeverages(random.Next(50, 100), service, nukaCola, fruits, variantsSoftDrink);

                service.AddProduct(new Product
                {
                    Name = "Terrazine",
                    Manufacturer = "Amon",
                    ModelNumber = "SC-002",
                    Description = "Intended for research.",
                    Stock = 321,
                    PackagingType = Product.Packaging.Shrinkwrap,
                    PriceNet = 600,
                    SubCategoryID = service.GetSubCategory("Non-carbonated soft drinks", "Non-alcoholic beverages").ID
                });
            }

            if (!context.Orders.Any())
            {
                service.AddOrder("John Smith", "37  Indiana Avenue, Honolulu, Hawaii, 96814", "36001111111", "john.smith@mailbucket.com", false, new List<Tuple<Product, int>> { new Tuple<Product, int>(context.Products.Single(p=>p.ID==1),10) });
                //service.AddOrder("Lois Woods", "44  Sigley Road, Asheville, North Carolina, 28803", "36002222222", "lois.woods@mailbucket.com", false, GetRandomProductOrders(context, 5));
                //service.AddOrder("Eva Holland", "61  Wiseman Street, Oak Ridge, Tennessee, 37830", "36003333333", "eva.holland@mailbucket.com", false, GetRandomProductOrders(context, 2));
            }
        }
    }
}
