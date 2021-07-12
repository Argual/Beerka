using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beerka.Web.Models;
using Beerka.Web.Services;
using Microsoft.AspNetCore.Http;

namespace Beerka.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BeerkaWebService _service;

        
        public HomeController(ILogger<HomeController> logger, BeerkaWebService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Index()
        {
            return View(_service.GetMainCategories());
        }

        public IActionResult GetSubCategoryProducts(int id)
        {
            return RedirectToAction("Index", "Products", new { id });
        }

        public IActionResult Result(string messageTitle=null, string messageSummary=null, string messageDetails = null)
        {
            if (string.IsNullOrEmpty(messageTitle))
            {
                messageTitle = "Result";
            }
            if (string.IsNullOrEmpty(messageSummary) && string.IsNullOrEmpty(messageDetails))
            {
                messageSummary = "This is not the page you are looking for!";
            }
            if (string.IsNullOrEmpty(messageDetails))
            {
                messageDetails = "";
            }
            ViewData["MessageSummary"] = messageSummary;
            ViewData["MessageDetails"] = messageDetails;
            ViewData["Title"] = messageTitle;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
