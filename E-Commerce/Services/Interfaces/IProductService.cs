using E_Commerce.Areas.Admin.ViewModels.ProductVM;
using E_Commerce.Models.Entities;

namespace E_Commerce.Services.Interfaces
{
    public interface IProductService
    {
        // Product
        Task CreateAsync(ProductCreateUpdateVM vm);
        Task UpdateAsync(ProductCreateUpdateVM vm);


        // GET
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int ProductId);
        Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, string? search, double? minPrice, double? maxPrice, string? sortPrice);
    }
}
