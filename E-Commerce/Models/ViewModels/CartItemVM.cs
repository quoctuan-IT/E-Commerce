namespace E_Commerce.Models.ViewModels
{
    public class CartItem
    {
        public int ProductId { get; set; }

        public required string ProductName { get; set; }

        public double UnitPrice { get; set; }

        public required string Image { get; set; }

        public int Quantity { get; set; }

        public double Total => Quantity * UnitPrice;
    }
}
