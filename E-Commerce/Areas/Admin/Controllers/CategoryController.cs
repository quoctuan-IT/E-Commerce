using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class CategoryController(ICategoryService categoryService) : Controller
    {
        private readonly ICategoryService _categoryService = categoryService;

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName")] Category newCategory)
        {
            if (!ModelState.IsValid) return View(newCategory);

            await _categoryService.CreateAsync(newCategory);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("CategoryName")] Category updatedCategory)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            if (!ModelState.IsValid) return View(updatedCategory);

            category.CategoryName = updatedCategory.CategoryName;
            await _categoryService.UpdateAsync(category);

            return RedirectToAction(nameof(Index));
        }
    }
}
