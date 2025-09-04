using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class SeedsController : Controller
    {
        private readonly ILogger<SeedsController> _logger;
        private readonly ISeedService _seedService;

        public SeedsController(ILogger<SeedsController> logger, ISeedService seedService)
        {
            _logger = logger;
            _seedService = seedService;

        }
        public async Task<IActionResult> Index()
        {
            var seeds = await _seedService.GetAllSeedsAsync();
            return View(seeds);
        }
        public async Task<IActionResult> Details(int id)
        {
            var seed = await _seedService.GetSeedByIdAsync(id);
            if (seed == null)
            {
                return NotFound();
            }
            return View(seed);
        }

        public IActionResult Create()
        { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seed seed)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _seedService.CreateSeedAsync(seed);
                    TempData["SuccessMessage"] = "Seed created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating seed: {ex.Message}");
                }
            }

            return View(seed);
        }

        // GET: Seeds/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var seed = await _seedService.GetSeedByIdAsync(id);
                if (seed == null)
                {
                    TempData["ErrorMessage"] = "Seed not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(seed);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading seed: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Seeds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seed seed)
        {
            if (id != seed.Id)
            {
                TempData["ErrorMessage"] = "Seed ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var rowsAffected = await _seedService.UpdateSeedAsync(seed);

                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Seed updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No changes were made to the seed.";
                        return View(seed);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating seed: {ex.Message}");
                    TempData["ErrorMessage"] = $"Error updating seed: {ex.Message}";
                }
            }

            // If we got this far, something failed; redisplay form
            return View(seed);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var seed = await _seedService.GetSeedByIdAsync(id);
            if (seed == null)
            {
                return NotFound();
            }
            return View(seed);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _seedService.DeleteSeedAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
