using E_Commerce.Areas.Admin.ViewModels.CategoryVM;

namespace E_Commerce.Areas.Admin.ViewModels.ProductVM
{
    public class ProductCreateUpdateVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public double UnitPrice { get; set; }

        public string? Description { get; set; }

        public IFormFile? UploadedImage { get; set; }

        public string? Image { get; set; }


        // IEnumerable
        public IEnumerable<CategoryItemVM> Categories { get; set; } = [];
    }
}
