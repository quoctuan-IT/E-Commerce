using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class ProductController(IProductService service) : Controller
    {
        private readonly IProductService _service = service;

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, string? search, double? minPrice, double? maxPrice, string? sortPrice)
        {
            var products = await _service.FilterProductsAsync(categoryId, search, minPrice, maxPrice, sortPrice);
            
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
    }
}
