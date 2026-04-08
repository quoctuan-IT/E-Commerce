namespace E_Commerce.Models.ViewModels.OrderVM
{
    public class OrderItemVM
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatusName { get; set; } = string.Empty;

        public string PaymentMethodName { get; set; } = string.Empty;

        public double TotalAmount { get; set; }

        public int TotalItems { get; set; }
    }
}
