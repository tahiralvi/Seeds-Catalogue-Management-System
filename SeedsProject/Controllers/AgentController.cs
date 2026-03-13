using Microsoft.AspNetCore.Mvc;
using SeedsProject.Models;
using SeedsProject.Services.Interface;

namespace SeedsProject.Controllers
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

        // GET: Agent/Index
        public async Task<IActionResult> Index()
        {
            // Fetch all agents using the implemented ADO.NET service
            var agents = await _agentService.GetAllAgentsAsync();
            return View(agents);
        }

        // GET: Agent/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Agent agent)
        {
            // The Seeds collection is not required for initial registration
            if (ModelState.IsValid)
            {
                try
                {
                    // Assign the current timestamp as the registration date
                    agent.CreatedDate = DateTime.Now;

                    // Call the service to perform the ADO.NET INSERT operation
                    int newId = await _agentService.CreateAgentAsync(agent);

                    if (newId > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during agent registration.");
                    ModelState.AddModelError("", "Unable to save changes. Try again later.");
                }
            }

            // If we got this far, something failed; redisplay the form with errors
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

        // GET: Agent/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var agent = await _agentService.GetAgentByIdAsync(id);
            if (agent == null)
            {
                return NotFound();
            }
            return View(agent);
        }

        // POST: Agent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _agentService.DeleteAgentAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting agent with ID {Id}", id);
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}