using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using FinalYearProject.Services.Model;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAgentService _agentService;

        public AccountController(ILogger<AccountController> logger, IAgentService agentService)
        {
            _logger = logger;
            _agentService = agentService;

        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Email,Phone")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                // Set the system-generated date
                agent.CreatedDate = DateTime.Now;

                await _agentService.CreateAgentAsync(agent);
                TempData["SuccessMessage"] = "Seed created successfully!";
                return RedirectToAction("Index", "Home");
            }
            return View(agent);
        }

    }
}
