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
    public class ProductsController : ControllerBase
    {
        private readonly BeerkaService _service;

        public ProductsController(BeerkaService service)
        {
            _service = service;
        }

        // GET: api/<ProductsController>
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<ProductDTO>> GetProducts()
        {
            return _service.GetProducts().Select(p => (ProductDTO)p).ToList();
        }

        // GET: api/<ProductsController>/1/2
        [Authorize]
        [HttpGet("{mainCategoryID}/{subCategoryID}")]
        public ActionResult<IEnumerable<ProductDTO>> GetProducts(int mainCategoryID, int subCategoryID)
        {
            if (!(_service.GetMainCategories().Any(c=>c.ID==mainCategoryID) && _service.GetSubCategories().Any(c => c.ID == subCategoryID)))
            {
                return NotFound();
            }

            return _service.GetProducts().Where(p=>p.SubCategoryID==subCategoryID).Select(p => (ProductDTO)p).ToList();
        }

        // GET api/<ProductsController>/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ProductDTO> GetProduct(int id)
        {
            Product product;
            try
            {
                product = _service.GetProduct(id);
            }
            catch (InvalidOperationException)
            {

                return NotFound();
            }
            return (ProductDTO)product;
        }

        // POST api/<ProductsController>
        [Authorize]
        [HttpPost]
        public ActionResult<ProductDTO> PostProduct(ProductDTO productDTO)
        {
            var product = (Product)productDTO;
            try
            {
                product = _service.AddProduct(product);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, (ProductDTO)product);
        }

        // PUT api/<ProductsController>/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, ProductDTO productDTO)
        {
            if (id != productDTO.ID)
            {
                return BadRequest();
            }

            if (_service.UpdateProduct((Product)productDTO))
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
