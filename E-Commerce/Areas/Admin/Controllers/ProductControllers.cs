using E_Commerce.Data;
using E_Commerce.Helpers;
using E_Commerce.Models;
using E_Commerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class ProductControllers(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var imagePath = await Utils.SaveImageAsync(vm.UploadedImage);

            var product = new Product
            {
                ProductName = vm.ProductName,
                CategoryId = vm.CategoryId,
                UnitPrice = vm.UnitPrice,
                Description = vm.Description,
                Image = imagePath
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = categories;
         
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var imagePath = await Utils.SaveImageAsync(vm.UploadedImage);

            product.ProductName = vm.ProductName;
            product.CategoryId = vm.CategoryId;
            product.UnitPrice = vm.UnitPrice;
            product.Description = vm.Description;
            product.Image = imagePath;

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
