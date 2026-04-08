using E_Commerce.Areas.Admin.ViewModels.CategoryVM;
using E_Commerce.Models.Entities;

namespace E_Commerce.Services.Interfaces
{
    public interface ICategoryService
    {
        // Category
        Task CreateAsync(CategoryCreateUpdateVM vm);
        Task UpdateAsync(CategoryCreateUpdateVM vm);


        // GET
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int categoryId);
    }
}
