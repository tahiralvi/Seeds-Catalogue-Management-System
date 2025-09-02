using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class SeedsController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public SeedsController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
