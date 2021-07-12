using Beerka.Persistence.DTO;
using Beerka.Persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beerka.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainCategoriesController : ControllerBase
    {
        private readonly BeerkaService _service;

        public MainCategoriesController(BeerkaService service)
        {
            _service = service;
        }

        // GET: api/<MainCategoriesController>
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<MainCategoryDTO>> GetMainCategories()
        {
            return _service.GetMainCategories().Select(c => (MainCategoryDTO)c).ToList();
        }

    }
}
