using E_Commerce.Models.ViewModels.CategoryVM;

namespace E_Commerce.Models.ViewModels.ProductVM
{
    public class ProductListVM
    {
        // Filter
        public int? CategoryId { get; set; }

        public string? Search { get; set; }

        public double? MinPrice { get; set; }

        public double? MaxPrice { get; set; }

        public string? SortPrice { get; set; }


        // IEnumerable
        public IEnumerable<ProductItemVM> Products { get; set; } = [];

        public IEnumerable<CategoryItemVM> Categories { get; set; } = [];
    }
}
