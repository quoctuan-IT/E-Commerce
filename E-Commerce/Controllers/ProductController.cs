using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ProductController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int idProduct)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .SingleOrDefaultAsync(p => p.ProductId == idProduct);

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByCategory(int idCategory)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == idCategory)
                .ToListAsync();

            return View("Index", products);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchKeyword)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.Contains(searchKeyword))
                .ToListAsync();

            return View("Index", products);
        }

        [HttpGet]
        public async Task<IActionResult> FilterPrice(string filterOption)
        {
            ViewData["FilterOption"] = "";

            var data = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(filterOption))
            {
                switch (filterOption)
                {
                    case "minToMaxPrice":
                        data = data.OrderBy(p => p.UnitPrice);
                        break;
                    case "maxToMinPrice":
                        data = data.OrderByDescending(p => p.UnitPrice);
                        break;
                }
            }

            var products = await data.ToListAsync();

            // Select - Option
            ViewData["FilterOption"] = filterOption;

            return View("Index", products);
        }
    }
}
