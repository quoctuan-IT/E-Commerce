using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services.Implementations
{
    public class CategoryService(AppDbContext context) : ICategoryService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _context.Categories.ToListAsync();

        public async Task<Category?> GetByIdAsync(int categoryId)
            => await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);

        public async Task CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
