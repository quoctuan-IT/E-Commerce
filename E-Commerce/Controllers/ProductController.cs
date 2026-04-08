using E_Commerce.Models.ViewModels.CategoryVM;
using E_Commerce.Models.ViewModels.ProductVM;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class ProductController(
        IProductService productService,
        ICategoryService categoryService
    ) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;


        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId,
            string? search,
            double? minPrice,
            double? maxPrice,
            string? sortPrice)
        {
            var products = await _productService.FilterProductsAsync(categoryId, search, minPrice, maxPrice, sortPrice);
            var categories = await _categoryService.GetAllAsync();

            ProductListVM vm = new()
            {
                CategoryId = categoryId,
                Search = search,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortPrice = sortPrice,

                Categories = categories.Select(c => new CategoryItemVM
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                }),

                Products = products.Select(p => new ProductItemVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    Description = p.Description,
                    Image = p.Image,

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.CategoryName ?? ""
                })
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null) return NotFound();

            ProductItemVM vm = new()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                Description = product.Description,
                Image = product.Image,

                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName
            };

            return View(vm);
        }
    }
}
