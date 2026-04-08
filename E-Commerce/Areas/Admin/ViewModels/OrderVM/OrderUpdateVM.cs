namespace E_Commerce.Areas.Admin.ViewModels.OrderVM
{
    public class OrderUpdateVM
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public int OrderStatusId { get; set; }

        public string? OrderStatusName { get; set; }

        public int PaymentMethodId { get; set; }


        // IEnumerable
        public IEnumerable<OrderStatusItemVM> OrderStatuses { get; set; } = [];
    }
}
