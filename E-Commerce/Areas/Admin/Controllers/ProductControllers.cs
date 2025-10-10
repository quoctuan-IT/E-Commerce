using E_Commerce.Helpers;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class ProductControllers(IProductService productService) : Controller
    {
        private readonly IProductService _productService = productService;

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();

            return View(products);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var imagePath = await Utils.SaveImageAsync(vm.UploadedImage);
            if (imagePath == null) return NotFound();

            var product = new Product
            {
                ProductName = vm.ProductName,
                CategoryId = vm.CategoryId,
                UnitPrice = vm.UnitPrice,
                Description = vm.Description,
                Image = imagePath
            };

            await _productService.CreateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //ViewData["categories"] = await _context.Categories.ToListAsync();

            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            var imagePath = await Utils.SaveImageAsync(vm.UploadedImage);
            if (imagePath == null) return NotFound();

            product.ProductName = vm.ProductName;
            product.CategoryId = vm.CategoryId;
            product.UnitPrice = vm.UnitPrice;
            product.Description = vm.Description;
            product.Image = imagePath;

            await _productService.UpdateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product != null)
            {
                await _productService.DeleteAsync(id);

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
