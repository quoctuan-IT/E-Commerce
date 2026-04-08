using E_Commerce.Areas.Admin.ViewModels.OrderVM;
using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels.CartVM;
using E_Commerce.Models.ViewModels.OrderVM;
using E_Commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services.Implementations
{
    public class OrderService(AppDbContext context) : IOrderService
    {
        private readonly AppDbContext _context = context;


        // Order
        public async Task<bool> CreateOrderAsync(string userId, CheckoutVM vm, List<CartItemVM> cartItems)
        {
            // Check Data
            if (cartItems == null || cartItems.Count == 0) return false;

            var isValidPayment = await _context.PaymentMethod.AnyAsync(p => p.PaymentMethodId == vm.PaymentMethodId);
            if (!isValidPayment) return false;

            var user = await _context.AppUser.SingleOrDefaultAsync(u => u.Id == userId)
                ?? throw new InvalidOperationException("User not found");

            var phoneNumber = vm.DefaultInformation ? user.PhoneNumber : vm.PhoneNumber;
            var address = vm.DefaultInformation ? user.Address : vm.Address;
            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phoneNumber)) return false;


            // Begin Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var productDict = await _context.Products
                    .Where(p => cartItems.Select(c => c.ProductId).Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.ProductId);

                var orderDetails = cartItems.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = productDict[item.ProductId].UnitPrice
                }).ToList();

                var total = orderDetails.Sum(x => x.Quantity * x.UnitPrice);

                // Create Order
                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.UtcNow,
                    Address = address,
                    PhoneNumber = phoneNumber,
                    PaymentMethodId = vm.PaymentMethodId,
                    TotalAmount = total,
                    OrderDetails = orderDetails
                };

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return false;
            }
        }

        public async Task UpdateAsync(OrderUpdateVM vm)
        {
            var order = await GetOrderByIdAsync(vm.OrderId);
            if (order == null) return;

            order.OrderStatusId = vm.OrderStatusId;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }


        // GET
        public async Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync()
            => await _context.OrderStatuses.ToListAsync();

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.OrderStatus)
            .Include(o => o.PaymentMethod)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.AppUser)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
