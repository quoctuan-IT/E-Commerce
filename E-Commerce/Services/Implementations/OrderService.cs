using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services.Implementations
{
    public class OrderService(AppDbContext context) : IOrderService
    {
        public async Task<IEnumerable<Order>> GetAllAsync()
            => await context.Orders.ToListAsync();

        public async Task<bool> CreateOrderAsync(int userId, CheckoutVM checkoutVM, List<CartItemVM> cartItems)
        {
            try
            {
                // Default address (optional)
                var user = new AppUser();
                if (checkoutVM.DefaultAddress)
                {
                    user = await context.AppUser.SingleOrDefaultAsync(u => u.UserId == userId);
                    if (user == null) throw new InvalidOperationException("User not found");
                }

                var order = new Order
                {
                    UserId = userId,
                    FullName = checkoutVM.FullName ?? user.FullName,
                    Address = checkoutVM.Address ?? user.Address,
                    Phone = checkoutVM.Phone ?? user.Phone,
                    PaymentMethod = checkoutVM.PaymentMethod,
                    ShippingMethod = checkoutVM.ShippingMethod,
                    OrderDate = DateTime.Now,
                    OrderStatusId = 1 // Assuming 1 is the default status (e.g., "Pending")
                };

                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    context.Add(order);
                    await context.SaveChangesAsync();

                    var orderDetails = new List<OrderDetail>();
                    foreach (var item in cartItems)
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            OrderId = order.OrderId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            ProductId = item.ProductId,
                        });
                    }

                    context.AddRange(orderDetails);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdateAsync(Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderStatus)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.AppUser)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
