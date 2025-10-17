using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
