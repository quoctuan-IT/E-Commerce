using E_Commerce.Models.Entities;

namespace E_Commerce.Service.Interfaces
{
    public interface IProductService
    {
        // ----- CLIENT -----
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, string? search, double? minPrice, double? maxPrice);
        Task<IEnumerable<Product>> SortProductsAsync(string? filterOption);

        // ----- ADMIN -----
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
