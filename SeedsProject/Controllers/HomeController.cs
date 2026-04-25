using Microsoft.AspNetCore.Mvc;
using SeedsProject.Models;
using SeedsProject.Services.Interface;
using SeedsProject.ViewModels;
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

        // Modernized home: returns featured seeds, product list and categories
        public async Task<IActionResult> Index()
        {
            var approvedSeeds = await _seedService.GetApprovedSeedsAsync() ?? new List<Seed>();
            var categories = await _seedService.GetAllCategoriesAsync() ?? new List<Category>();

            var featured = approvedSeeds
                .OrderByDescending(s => s.CreatedDate)
                .Take(5)
                .ToList();

            var popular = approvedSeeds
                .OrderByDescending(s => s.Stock)      // simple heuristic: high stock as "popular" for demo
                .Take(6)
                .ToList();

            var model = new HomeIndexViewModel
            {
                Featured = featured,
                Popular = popular,
                Seeds = approvedSeeds,
                Categories = categories
            };

            return View(model);
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