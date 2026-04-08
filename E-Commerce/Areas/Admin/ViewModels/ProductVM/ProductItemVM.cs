namespace E_Commerce.Areas.Admin.ViewModels.ProductVM
{
    public class ProductItemVM
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public double UnitPrice { get; set; }

        public string? Description { get; set; }

        public string Image { get; set; } = null!;


        public int CategoryId { get; set; }

        public string? CategoryName { get; set; } = null!;
    }
}
