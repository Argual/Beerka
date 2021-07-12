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
    public class SubCategoriesControllerTest : IDisposable
    {
        private readonly BeerkaContext _context;
        private readonly BeerkaService _service;
        private readonly SubCategoriesController _controller;

        public SubCategoriesControllerTest()
        {
            var options = new DbContextOptionsBuilder<BeerkaContext>()
                .UseInMemoryDatabase("TestDb_SubCategoriesControllerTest")
                .Options;

            _context = new BeerkaContext(options);
            TestDbInitializer.Initialize(_context);
            _service = new BeerkaService(_context);
            _controller = new SubCategoriesController(_service);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetSubCategoriesTest(int id)
        {
            // Act
            var result = _controller.GetSubCategories(id);

            // Assert
            var content = Assert.IsAssignableFrom<IEnumerable<SubCategoryDTO>>(result.Value);
            Assert.Equal(2, content.Count());
            Assert.True(content.All(c => c.MainCategoryID == id));
        }

        [Fact]
        public void GetInvalidSubCategoriesTest()
        {
            // Arrange
            var id = 5;

            // Act
            var result = _controller.GetSubCategories(id);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }
    }
}
