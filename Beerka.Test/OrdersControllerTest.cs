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
    public class OrdersControllerTest : IDisposable
    {
        private readonly BeerkaContext _context;
        private readonly BeerkaService _service;
        private readonly OrdersController _controller;

        public OrdersControllerTest()
        {
            var options = new DbContextOptionsBuilder<BeerkaContext>()
                .UseInMemoryDatabase("TestDb_OrdersControllerTest")
                .Options;

            _context = new BeerkaContext(options);
            TestDbInitializer.Initialize(_context);
            _service = new BeerkaService(_context);
            _controller = new OrdersController(_service);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetOrdersTest()
        {
            // Act
            var result = _controller.GetOrders();

            // Assert
            var content = Assert.IsAssignableFrom<IEnumerable<OrderDTO>>(result.Value);
            Assert.Equal(2, content.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetOrderTest(int id)
        {
            // Act
            var result = _controller.GetOrder(id);

            // Assert
            var content = Assert.IsAssignableFrom<OrderDTO>(result.Value);
            Assert.Equal(content.ID, id);
        }

        [Fact]
        public void GetInvalidOrderTest()
        {
            // Arrange
            int id = 4;

            // Act
            var result = _controller.GetOrder(id);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public void PutOrderTest()
        {
            // Arrange
            var updatedOrder = _context.Orders.Single(o => o.ID == 1);
            string origCustomerName = updatedOrder.CustomerName;
            updatedOrder.CustomerName += " Updated";

            // Act
            var result = _controller.PutOrder(1, (OrderDTO)updatedOrder);

            // Assert
            Assert.Equal(origCustomerName+" Updated",_context.Orders.Single(o => o.ID == 1).CustomerName);
        }

        [Fact]
        public void PutInvalidOrderTest()
        {
            // Arrange
            var updatedOrder = _context.Orders.Single(o => o.ID == 1);
            string origCustomerName = updatedOrder.CustomerName;
            updatedOrder.CustomerName += " Updated";

            // Act
            var result = _controller.PutOrder(3, (OrderDTO)updatedOrder);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }
    }
}
