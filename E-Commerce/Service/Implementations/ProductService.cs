using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Service.Implementations
{
    public class ProductService(AppDbContext context) : IProductService
    {
        private readonly AppDbContext _context = context;

        // ========================
        // CLIENT SECTION
        // ========================
        public async Task<IEnumerable<Product>> GetAllAsync()
            => await _context.Products.Include(p => p.Category).ToListAsync();

        public async Task<Product?> GetByIdAsync(int id)
            => await _context.Products.Include(p => p.Category)
                                      .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, string? search, double? minPrice, double? maxPrice)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.ProductName.Contains(search));

            if (minPrice.HasValue)
                query = query.Where(p => p.UnitPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.UnitPrice <= maxPrice.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> SortProductsAsync(string? filterOption)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(filterOption))
            {
                switch (filterOption)
                {
                    case "minToMaxPrice":
                        query = query.OrderBy(p => p.UnitPrice);
                        break;
                    case "maxToMinPrice":
                        query = query.OrderByDescending(p => p.UnitPrice);
                        break;
                }
            }

            return await query.ToListAsync();
        }

        // ========================
        // ADMIN SECTION
        // ========================
        public async Task CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }

}
