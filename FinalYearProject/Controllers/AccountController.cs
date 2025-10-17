using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
