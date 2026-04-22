using E_Commerce.Models.DTOs;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProducts(
            [FromQuery] int? categoryId,
            [FromQuery] string? search,
            [FromQuery] double? minPrice,
            [FromQuery] double? maxPrice,
            [FromQuery] string? sortPrice)
        {
            var products = await _productService.FilterProductsAsync(
                categoryId,
                search,
                minPrice,
                maxPrice,
                sortPrice
            );

            var response = products.Select(p => new ProductResponseDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName ?? string.Empty,
                UnitPrice = p.UnitPrice,
                Image = p.Image,
                Description = p.Description,
                IsActive = p.IsActive
            });

            return Ok(response);
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<ProductResponseDTO>> GetProductById(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null) return NotFound(new { message = "Product not found" });

            return Ok(new ProductResponseDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName ?? string.Empty,
                UnitPrice = product.UnitPrice,
                Image = product.Image,
                Description = product.Description,
                IsActive = product.IsActive
            });
        }
    }
}
