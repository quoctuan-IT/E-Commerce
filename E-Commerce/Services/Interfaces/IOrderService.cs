using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;

namespace E_Commerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<List<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<bool> CreateOrderAsync(int userId, CheckoutVM checkoutVM, List<CartItemVM> cartItems);
        Task UpdateAsync(Order order);
    }
}
