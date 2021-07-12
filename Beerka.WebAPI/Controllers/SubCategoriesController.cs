using Beerka.Persistence.Services;
using Beerka.Persistence;
using Beerka.Persistence.DTO;
using Microsoft.AspNetCore.Authorization;
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
    public class SubCategoriesController : ControllerBase
    {
        private readonly BeerkaService _service;

        public SubCategoriesController(BeerkaService service)
        {
            _service = service;
        }

        // GET: api/<SubCategoriesController>/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<SubCategoryDTO>> GetSubCategories(int id)
        {
            if (!_service.GetSubCategories().Any(c=>c.ID==id))
            {
                return NotFound();
            }

            return _service.GetSubCategories().Where(c=>c.MainCategoryID==id).Select(c => (SubCategoryDTO)c).ToList();
        }

    }
}
