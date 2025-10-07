using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ProductController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        private async Task LoadCategory()
        {
            var categories = await _context.Categories.ToListAsync();

            // Keep key name to avoid view changes for now
            ViewData["Loais"] = categories;
        }

        public async Task<IActionResult> Index()
        {
            // List Danh Mục.
            await LoadCategory();

            // List Sản Phẩm.
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> FilterByCategory(int idCategory)
        {
            // List Danh Mục.
            await LoadCategory();

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == idCategory)
                .ToListAsync();

            return View("Index", products);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchKeyword)
        {
            // List Danh Mục.
            await LoadCategory();

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.Contains(searchKeyword))
                .ToListAsync();

            return View("Index", products);
        }

        public async Task<IActionResult> Detail(int idProduct)
        {
            // List Danh Mục.
            await LoadCategory();

            var product = await _context.Products
                .Include(p => p.Category)
                .SingleOrDefaultAsync(p => p.ProductId == idProduct);

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> FilterPrice(string filterOption)
        {
            // Lưu giá trị filterOption để giữ trạng thái dropdown
            ViewData["FilterOption"] = "";

            // List Danh Mục.
            await LoadCategory();

            // List Sản Phẩm.
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            // Áp dụng bộ lọc theo tùy chọn
            if (!string.IsNullOrEmpty(filterOption))
            {
                switch (filterOption)
                {
                    case "minToMaxPrice":
                        productsQuery = productsQuery.OrderBy(p => p.UnitPrice);
                        break;
                    case "maxToMinPrice":
                        productsQuery = productsQuery.OrderByDescending(p => p.UnitPrice);
                        break;
                }
            }

            var products = await productsQuery.ToListAsync();

            ViewData["FilterOption"] = filterOption;

            return View("Index", products);
        }
    }
}
