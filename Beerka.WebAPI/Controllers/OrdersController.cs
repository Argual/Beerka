using Beerka.Persistence;
using Beerka.Persistence.DTO;
using Beerka.Persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Beerka.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BeerkaService _service;

        public OrdersController(BeerkaService service)
        {
            _service = service;
        }

        // GET: api/<OrdersController>
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<OrderDTO>> GetOrders()
        {
            return _service.GetOrders().Select(o => (OrderDTO)o).ToList();
        }

        // GET api/<OrdersController>/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<OrderDTO> GetOrder(int id)
        {
            Order order;
            try
            {
                order = _service.GetOrder(id);
            }
            catch (InvalidOperationException)
            {

                return NotFound();
            }
            return (OrderDTO)order;
        }

        // PUT api/<OrdersController>/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult PutOrder(int id, OrderDTO orderDTO)
        {
            if (id != orderDTO.ID)
            {
                return BadRequest();
            }

            if (_service.UpdateOrderDeliveryStatus((Order)orderDTO))
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
