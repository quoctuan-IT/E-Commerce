using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels.CartVM;
using E_Commerce.Models.ViewModels.OrderVM;
using E_Commerce.Areas.Admin.ViewModels.OrderVM;

namespace E_Commerce.Services.Interfaces
{
    public interface IOrderService
    {
        // Order
        Task<bool> CreateOrderAsync(string userId, CheckoutVM checkoutVM, List<CartItemVM> cartItems);
        Task UpdateAsync(OrderUpdateVM vm);
        Task DeleteOrderAsync(int orderId);


        // GET
        Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync();
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
