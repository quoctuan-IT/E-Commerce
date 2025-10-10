using E_Commerce.Models.Entities;

namespace E_Commerce.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, string? search, double? minPrice, double? maxPrice, string? sortPrice);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
