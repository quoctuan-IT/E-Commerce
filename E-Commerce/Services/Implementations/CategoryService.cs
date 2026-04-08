using E_Commerce.Areas.Admin.ViewModels.CategoryVM;
using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services.Implementations
{
    public class CategoryService(AppDbContext context) : ICategoryService
    {
        private readonly AppDbContext _context = context;


        // GET
        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _context.Categories.ToListAsync();

        public async Task<Category?> GetByIdAsync(int categoryId)
            => await _context.Categories.FindAsync(categoryId);


        // Category
        public async Task CreateAsync(CategoryCreateUpdateVM vm)
        {
            var category = new Category
            {
                CategoryName = vm.CategoryName,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryCreateUpdateVM vm)
        {
            var category = await GetByIdAsync(vm.CategoryId);
            if (category == null) return;

            category.CategoryName = vm.CategoryName;

            await _context.SaveChangesAsync();
        }
    }
}
