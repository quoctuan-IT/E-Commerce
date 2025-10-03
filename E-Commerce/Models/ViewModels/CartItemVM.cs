namespace E_Commerce.Models.ViewModels
{
    public class CartItemVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public double UnitPrice { get; set; }

        public string Image { get; set; } = null!;

        public int Quantity { get; set; }

        public double Total => Quantity * UnitPrice;
    }
}
