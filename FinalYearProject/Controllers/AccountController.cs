using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async IActionResult Register(SeedAgentCreateViewModel viewModel)
        {
            return View(viewModel);
        }

    }
}
