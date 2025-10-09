using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Commerce.Controllers
{
    public class ProductController(IProductService service) : Controller
    {
        private readonly IProductService _service = service;

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, string? search, double? minPrice, double? maxPrice)
        {
            var products = await _service.FilterProductsAsync(categoryId, search, minPrice, maxPrice);
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> FilterPrice(string? filterOption)
        {
            var products = await _service.SortProductsAsync(filterOption);
            ViewData["FilterOption"] = filterOption;
            return View("Index", products);
        }
    }
}
