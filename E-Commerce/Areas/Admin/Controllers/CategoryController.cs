using E_Commerce.Areas.Admin.ViewModels.CategoryVM;
using E_Commerce.Helpers;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController(ICategoryService categoryService) : Controller
    {
        private readonly ICategoryService _categoryService = categoryService;


        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();

            List<CategoryItemVM> vm = [.. categories.Select(c => new CategoryItemVM
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            })];

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _categoryService.CreateAsync(vm);
            TempData["Success"] = "Create Category successful!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int categoryId)
        {
            var category = await _categoryService.GetByIdAsync(categoryId);
            if (category == null) return NotFound();

            CategoryCreateUpdateVM vm = new()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryCreateUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _categoryService.UpdateAsync(vm);
            TempData["Success"] = "Update Category successful!";

            return RedirectToAction(nameof(Index));
        }
    }
}
