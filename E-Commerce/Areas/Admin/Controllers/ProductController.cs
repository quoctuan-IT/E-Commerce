using E_Commerce.Areas.Admin.ViewModels.ProductVM;
using E_Commerce.Areas.Admin.ViewModels.CategoryVM;
using E_Commerce.Helpers;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class ProductController(
        IProductService productService,
        ICategoryService categoryService
    ) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;


        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();

            List<ProductItemVM> vm = [.. products.Select(p => new ProductItemVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                Description = p.Description,
                Image = p.Image,

                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName ?? ""
            })];

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync();
            if (categories == null) return NotFound();

            ProductCreateUpdateVM vm = new()
            {
                ProductName = "",

                Categories = categories.Select(c => new CategoryItemVM
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                }),
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _productService.CreateAsync(vm);
            TempData["Success"] = "Create Product successful!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            var categories = await _categoryService.GetAllAsync();
            if (product == null || categories == null) return NotFound();

            ProductCreateUpdateVM vm = new()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                UnitPrice = product.UnitPrice,
                Description = product.Description,
                Image = product.Image,

                Categories = categories.Select(c => new CategoryItemVM
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                }),
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductCreateUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _productService.UpdateAsync(vm);
            TempData["Success"] = "Update Product successful!";

            return RedirectToAction(nameof(Index));
        }
    }
}
