using Beerka.Persistence;
using Beerka.Persistence.DTO;
using Beerka.Persistence.Services;
using Beerka.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Beerka.Test
{
    public class ProductsControllerTest : IDisposable
    {
        private readonly BeerkaContext _context;
        private readonly BeerkaService _service;
        private readonly ProductsController _controller;

        public ProductsControllerTest()
        {
            var options = new DbContextOptionsBuilder<BeerkaContext>()
                .UseInMemoryDatabase("TestDb_ProductsControllerTest")
                .Options;

            _context = new BeerkaContext(options);
            TestDbInitializer.Initialize(_context);
            _service = new BeerkaService(_context);
            _controller = new ProductsController(_service);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetProductsTest()
        {
            // Act
            var result = _controller.GetProducts();

            // Assert
            var content = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(result.Value);
            Assert.Equal(_context.Products.Count(), content.Count());
        }

        [Theory]
        [InlineData(1,1)]
        public void GetProductsByCategoryIDsTest(int mainCategoryID, int subCategoryID)
        {
            // Act
            var result = _controller.GetProducts(mainCategoryID, subCategoryID);

            // Assert
            var correct = _context.Products.Where(p => p.SubCategoryID == subCategoryID).Select(p => (ProductDTO)p).ToList();
            Assert.Equal(result.Value.ToList().Count(), correct.Count());
            Assert.True(result.Value.ToList().All(p=>correct.Select(x=>x.ID).Contains(p.ID)));
        }

        [Theory]
        [InlineData(0, 0)]
        public void GetInvalidProductsByCategoryIDsTest(int mainCategoryID, int subCategoryID)
        {
            // Act
            var result = _controller.GetProducts(mainCategoryID, subCategoryID);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetProductByIDTest()
        {
            // Arrange
            int id = 1;

            // Act
            var result = _controller.GetProduct(id);

            // Assert
            Assert.Equal(id, result.Value.ID);

        }

        [Fact]
        public void GetInvalidProductByIDTest()
        {
            // Arrange
            int id = 6;

            // Act
            var result = _controller.GetProduct(id);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public void PostProductTest()
        {
            // Arrange
            var product = new ProductDTO {
                Name="New Product",
                Description="D",
                Manufacturer="M",
                ModelNumber="m0",
                PackagingTypeString=null,
                PriceNet=100,
                Stock=100,
                SubCategoryID=1
            };
            int count = _context.Products.Count();

            // Act
            var result = _controller.PostProduct(product);

            // Assert
            var objectResult = Assert.IsAssignableFrom<CreatedAtActionResult>(result.Result);
            var content = Assert.IsAssignableFrom<ProductDTO>(objectResult.Value);
            Assert.Equal(count + 1, _context.Products.Count());
        }

        [Fact]
        public void PutProductTest()
        {
            // Arrange
            var product = _context.Products.Single(predicate => predicate.ID == 1);
            string originalName = product.Name;
            product.Name += " Updated";

            // Act
            var result = _controller.PutProduct(1, (ProductDTO)product);

            // Assert
            Assert.Equal(_context.Products.Single(p=>p.ID==1).Name, originalName + " Updated");
        }

        [Fact]
        public void PutInvalidProductTest()
        {
            // Arrange
            var product = _context.Products.Single(predicate => predicate.ID == 1);

            // Act
            var result = _controller.PutProduct(0, (ProductDTO)product);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

    }
}
