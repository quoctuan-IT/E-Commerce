namespace E_Commerce.Models.ViewModels.OrderVM;

public class OrderVM
{
    public int TotalOrders { get; set; }


    public IEnumerable<OrderItemVM> Orders { get; set; } = [];
}