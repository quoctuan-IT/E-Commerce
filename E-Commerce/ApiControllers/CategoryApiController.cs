using E_Commerce.Models.DTOs;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryApiController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            var response = categories.Select(c => new CategoryResponseDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            });

            return Ok(response);
        }

        [HttpGet("{categoryId:int}")]
        public async Task<ActionResult<CategoryResponseDTO>> GetCategoryById(int categoryId)
        {
            var category = await _categoryService.GetByIdAsync(categoryId);
            if (category == null) return NotFound(new { message = "Category not found" });

            return Ok(new CategoryResponseDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            });
        }
    }
}
