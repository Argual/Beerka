using Beerka.Persistence;
using Beerka.Persistence.DTO;
using Beerka.Persistence.Services;
using Beerka.WebAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Beerka.Test
{
    public class MainCategoriesControllerTest : IDisposable
    {
        private readonly BeerkaContext _context;
        private readonly BeerkaService _service;
        private readonly MainCategoriesController _controller;

        public MainCategoriesControllerTest()
        {
            var options = new DbContextOptionsBuilder<BeerkaContext>()
                .UseInMemoryDatabase("TestDb_MainCategoriesControllerTest")
                .Options;

            _context = new BeerkaContext(options);
            TestDbInitializer.Initialize(_context);
            _service = new BeerkaService(_context);
            _controller = new MainCategoriesController(_service);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetMainCategoriesTest()
        {
            // Act
            var result = _controller.GetMainCategories();

            // Assert
            var content = Assert.IsAssignableFrom<IEnumerable<MainCategoryDTO>>(result.Value);
            Assert.Equal(2, content.Count());
        }
    }
}
