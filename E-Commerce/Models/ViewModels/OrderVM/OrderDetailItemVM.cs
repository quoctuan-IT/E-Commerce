namespace E_Commerce.Models.ViewModels.OrderVM
{
    public class OrderDetailItemVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double SubTotal => UnitPrice * Quantity;
    }
}
