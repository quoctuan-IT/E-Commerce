using E_Commerce.Models;
using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services.Implementations
{
    public class OrderService(AppDbContext context) : IOrderService
    {
        public async Task<IEnumerable<Order>> GetAllAsync() => await context.Orders.ToListAsync();

        public async Task<bool> CreateOrderAsync(string userId, CheckoutVM checkoutVM, List<CartItemVM> cartItems)
        {
            try
            {
                // Default address (optional)
                var user = new AppUser();
                if (checkoutVM.DefaultAddress)
                {
                    user = await context.AppUser.SingleOrDefaultAsync(u => u.Id == userId);
                    if (user == null) throw new InvalidOperationException("User not found");
                }

                var order = new Order
                {
                    UserId = userId,
                    Address = checkoutVM.Address,
                    Phone = checkoutVM.Phone,
                    PaymentMethod = checkoutVM.PaymentMethod,
                    OrderDate = DateTime.Now,
                    OrderStatusId = 1
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

        public async Task<bool> CreateOrderFromApiAsync(string userId, CreateOrderDto createOrderDto)
        {
            try
            {
                // Default address (optional)
                var user = new AppUser();
                if (createOrderDto.DefaultAddress)
                {
                    user = await context.AppUser.SingleOrDefaultAsync(u => u.Id == userId);
                    if (user == null) throw new InvalidOperationException("User not found");
                }

                var order = new Order
                {
                    UserId = userId,
                    Address = createOrderDto.Address,
                    Phone = createOrderDto.Phone,
                    PaymentMethod = createOrderDto.PaymentMethod,
                    OrderDate = DateTime.Now,
                    OrderStatusId = 1
                };

                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    context.Add(order);
                    await context.SaveChangesAsync();

                    var orderDetails = new List<OrderDetail>();
                    foreach (var item in createOrderDto.Items)
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

        public async Task<bool> UpdateOrderAsync(int orderId, UpdateOrderDto updateOrderDto)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);
                if (order == null) return false;

                if (!string.IsNullOrEmpty(updateOrderDto.Address))
                    order.Address = updateOrderDto.Address;

                if (!string.IsNullOrEmpty(updateOrderDto.Phone))
                    order.Phone = updateOrderDto.Phone;

                if (!string.IsNullOrEmpty(updateOrderDto.PaymentMethod))
                    order.PaymentMethod = updateOrderDto.PaymentMethod;

                if (updateOrderDto.OrderStatusId.HasValue)
                    order.OrderStatusId = updateOrderDto.OrderStatusId.Value;

                context.Orders.Update(order);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);
                if (order == null) return false;

                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersWithFilterAsync(OrderFilterDto filter)
        {
            var query = context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.AppUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(o => o.UserId == filter.UserId);

            if (filter.OrderStatusId.HasValue)
                query = query.Where(o => o.OrderStatusId == filter.OrderStatusId.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(o => o.OrderDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(o => o.OrderDate <= filter.EndDate.Value);

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
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
