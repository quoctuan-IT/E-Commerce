using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetProducts([FromQuery] ProductFilterDto filter)
        {
            try
            {
                var products = await _productService.FilterProductsAsync(
                    filter.CategoryId, 
                    filter.Search, 
                    filter.MinPrice, 
                    filter.MaxPrice, 
                    filter.SortPrice);

                var totalCount = products.Count();
                var pagedProducts = products
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(MapToDto);

                var result = new PagedResultDto<ProductDto>
                {
                    Data = pagedProducts,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null) return NotFound(new { message = "Product not found" });

                return Ok(MapToDto(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var product = new Product
                {
                    ProductName = createDto.ProductName,
                    Description = createDto.Description,
                    UnitPrice = createDto.UnitPrice,
                    Image = createDto.Image ?? "",
                    CategoryId = createDto.CategoryId,
                    IsActive = createDto.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _productService.CreateAsync(product);

                var createdProduct = await _productService.GetByIdAsync(product.ProductId);

                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, MapToDto(createdProduct!));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var existingProduct = await _productService.GetByIdAsync(id);
                if (existingProduct == null) return NotFound(new { message = "Product not found" });

                existingProduct.ProductName = updateDto.ProductName;
                existingProduct.Description = updateDto.Description;
                existingProduct.UnitPrice = updateDto.UnitPrice;
                existingProduct.Image = updateDto.Image ?? "";
                existingProduct.CategoryId = updateDto.CategoryId;
                existingProduct.IsActive = updateDto.IsActive;
                existingProduct.UpdatedDate = DateTime.Now;

                await _productService.UpdateAsync(existingProduct);

                var updatedProduct = await _productService.GetByIdAsync(id);

                return Ok(MapToDto(updatedProduct!));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null) return NotFound(new { message = "Product not found" });

                await _productService.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.FilterProductsAsync(categoryId, null, null, null, null);
                var productDtos = products.Select(MapToDto);
                
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return BadRequest(new { message = "Search query cannot be empty" });

                var products = await _productService.FilterProductsAsync(null, query, null, null, null);
                var productDtos = products.Select(MapToDto);

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByPriceRange([FromQuery] double minPrice, [FromQuery] double maxPrice)
        {
            try
            {
                var products = await _productService.FilterProductsAsync(null, null, minPrice, maxPrice, null);
                var productDtos = products.Select(MapToDto);

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                var productDtos = products.Select(MapToDto);

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                Image = product.Image,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                IsActive = product.IsActive
            };
        }
    }
}
