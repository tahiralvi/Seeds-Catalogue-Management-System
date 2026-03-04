using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class AgentController : Controller
    {
        private readonly ILogger<AgentController> _logger;
        private readonly IAgentService _agentService;

        public AgentController(ILogger<AgentController> logger, IAgentService agentService)
        {
            _logger = logger;
            _agentService = agentService;

        }

        // GET: Account/Index
        public async Task<IActionResult> Index()
        {
            // Fetch all agents using the implemented ADO.NET service
            var agents = await _agentService.GetAllAgentsAsync();
            return View(agents);
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Agent agent)
        {
            if (ModelState.IsValid)
            {
                agent.CreatedDate = DateTime.Now;
                int newId = await _agentService.CreateAgentAsync(agent);

                if (newId > 0) return RedirectToAction("Index", "Home");
            }
            return View(agent);
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var agent = await _agentService.GetAgentWithDetailsAsync(id);

            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

    }
}
