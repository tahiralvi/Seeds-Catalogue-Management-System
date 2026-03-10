using Microsoft.AspNetCore.Mvc;
using SeedsProject.Models;
using SeedsProject.Services.Interface;

namespace SeedsProject.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ILogger<SeedsController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService, ILogger<SeedsController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _categoryService.GetAllCategoryAsync();
            _logger.Log(LogLevel.Information, $"List Count {list.Count}", list);
            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _categoryService.GetCategoryByIdAsync(id);
            return cat == null ? NotFound() : View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Added: View details of a single category
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return category == null ? NotFound() : View(category);
        }
    }
}