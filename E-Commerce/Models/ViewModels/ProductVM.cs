namespace E_Commerce.Models.ViewModels
{
    public class ProductVM
    {
        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public double UnitPrice { get; set; }

        public string? Description { get; set; }

        public IFormFile? UploadedImage { get; set; }
    }
}
