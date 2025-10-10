using E_Commerce.Models.Entities;

namespace E_Commerce.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int categoryId);
        Task CreateAsync(Category category);
        Task UpdateAsync(Category category);
    }
}
