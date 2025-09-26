using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> Index(string searchTerm, string approvalStatus,
                                         int? minStock, int? maxStock)
        {
            try
            {
                // Parse approval status
                bool? approvalStatusValue = null;
                if (!string.IsNullOrEmpty(approvalStatus))
                {
                    approvalStatusValue = bool.Parse(approvalStatus);
                }

                // Get filtered seeds
                var seeds = await _seedService.GetSeedsAsync(
                    searchTerm: searchTerm,
                    approvalStatus: approvalStatusValue,
                    minStock: minStock,
                    maxStock: maxStock
                );

                // Pass search parameters to view for maintaining filter state
                ViewBag.SearchTerm = searchTerm;
                ViewBag.ApprovalStatus = approvalStatus;
                ViewBag.MinStock = minStock;
                ViewBag.MaxStock = maxStock;

                return View(seeds);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading seeds: {ex.Message}";
                return View(new List<Seed>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var seed = await _seedService.GetSeedWithDetailsAsync(id);
                if (seed == null)
                {
                    TempData["ErrorMessage"] = "Seed not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(seed);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading seed details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Controllers/SeedsController.cs
        // GET: Seeds/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new SeedCreateViewModel
            {
                ExpiryDate = DateTime.Today.AddMonths(6),
                Approval = false,
                Stock = 0,
                Categories = await _seedService.GetAllCategoriesAsync(),
                Agents = await _seedService.GetAllAgentsAsync()
            };

            return View(viewModel);
        }

        // POST: Seeds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeedCreateViewModel viewModel)
        {
            // Remove navigation properties from ModelState validation
            ModelState.Remove("Categories");
            ModelState.Remove("Agents");

            if (ModelState.IsValid)
            {
                try
                {
                    // Convert ViewModel to Entity
                    var seed = new Seed
                    {
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        Price = viewModel.Price,
                        Approval = viewModel.Approval,
                        Stock = viewModel.Stock,
                        Image = viewModel.Image,
                        ExpiryDate = viewModel.ExpiryDate,
                        AgentID = viewModel.AgentID,
                        CategoryID = viewModel.CategoryID,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    await _seedService.CreateSeedAsync(seed);
                    TempData["SuccessMessage"] = "Seed created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating seed: {ex.Message}");
                }
            }

            // Repopulate dropdown data
            viewModel.Categories = await _seedService.GetAllCategoriesAsync();
            viewModel.Agents = await _seedService.GetAllAgentsAsync();

            return View(viewModel);
        }

        // GET: Seeds/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            await PopulateViewData();

            try
            {
                var seed = await _seedService.GetSeedWithDetailsAsync(id);
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

            // Remove navigation properties from ModelState validation
            ModelState.Remove("Category");
            ModelState.Remove("Agent");

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

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var seed = await _seedService.GetSeedByIdAsync(id);
        //    if (seed == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(seed);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    await _seedService.DeleteSeedAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}

        #region Private methods
        private async Task PopulateViewData()
        {
            var categories = await _seedService.GetAllCategoriesAsync();
            var agents = await _seedService.GetAllAgentsAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Agents = new SelectList(agents, "Id", "Name");
        }
        #endregion
    }
}
