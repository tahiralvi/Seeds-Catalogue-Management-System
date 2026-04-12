using Microsoft.AspNetCore.Mvc;
using SeedsProject.Models;
using SeedsProject.Services.Interface;
using System.Diagnostics;

namespace SeedsProject.Controllers  
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISeedService _seedService;

        public HomeController(ILogger<HomeController> logger, ISeedService seedService)
        {
            _logger = logger;
            _seedService = seedService;
        }

        public async Task<IActionResult> Index()
        {
            // Get approved seeds for public listing / carousel
            var seeds = await _seedService.GetApprovedSeedsAsync() ?? new List<Seed>();
            return View(seeds);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}