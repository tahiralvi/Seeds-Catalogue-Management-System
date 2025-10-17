using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class AgentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
