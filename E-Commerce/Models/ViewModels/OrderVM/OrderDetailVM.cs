namespace E_Commerce.Models.ViewModels.OrderVM
{
    public class OrderDetailVM
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string OrderStatusName { get; set; } = string.Empty;

        public string PaymentMethodName { get; set; } = string.Empty;

        public double TotalAmount { get; set; }


        public IEnumerable<OrderDetailItemVM> Items { get; set; } = [];
    }
}
