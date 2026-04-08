using E_Commerce.Areas.Admin.ViewModels.ProductVM;
using E_Commerce.Helpers;
using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services.Implementations
{
    public class ProductService(AppDbContext context) : IProductService
    {
        private readonly AppDbContext _context = context;


        // Product
        public async Task<IEnumerable<Product>> GetAllAsync()
            => await _context.Products.Include(p => p.Category).ToListAsync();

        public async Task<Product?> GetByIdAsync(int productId)
            => await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == productId);

        public async Task<IEnumerable<Product>> FilterProductsAsync(
            int? categoryId,
            string? search,
            double? minPrice,
            double? maxPrice,
            string? sortPrice)
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

            if (!string.IsNullOrWhiteSpace(sortPrice))
            {
                if (sortPrice == "sortMinToMaxPrice")
                    query = query.OrderBy(p => p.UnitPrice);
                else if (sortPrice == "sortMaxToMinPrice")
                    query = query.OrderByDescending(p => p.UnitPrice);
            }

            return await query.ToListAsync();
        }


        // Product
        public async Task CreateAsync(ProductCreateUpdateVM vm)
        {
            var imagePath = await Utils.SaveImageAsync(vm.UploadedImage);
            if (imagePath == null) return;

            var product = new Product
            {
                ProductName = vm.ProductName,
                CategoryId = vm.CategoryId,
                UnitPrice = vm.UnitPrice,
                Description = vm.Description,
                Image = imagePath
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductCreateUpdateVM vm)
        {
            var product = await GetByIdAsync(vm.ProductId);
            if (product == null) return;

            if (vm.UploadedImage != null && vm.UploadedImage.Length > 0)
            {
                var imagePath = await Utils.SaveImageAsync(vm.UploadedImage);
                if (imagePath != null)
                {
                    product.Image = imagePath;
                }
            }

            product.ProductName = vm.ProductName;
            product.CategoryId = vm.CategoryId;
            product.UnitPrice = vm.UnitPrice;
            product.Description = vm.Description;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
