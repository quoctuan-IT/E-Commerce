using E_Commerce.Models;
using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderApiController(
        AppDbContext context,
        IAccountService accountService
    ) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IAccountService _accountService = accountService;


        [HttpPost("direct-checkout")]
        public async Task<ActionResult<OrderResponseDTO>> DirectCheckout([FromBody] DirectCheckoutRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = _accountService.GetCurrentUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized(new { message = "User not found" });

            var isPaymentMethodValid = await _context.PaymentMethod
                .AnyAsync(x => x.PaymentMethodId == request.PaymentMethodId);
            if (!isPaymentMethodValid)
                return BadRequest(new { message = "Invalid payment method" });

            var normalizedItems = request.Items
                .GroupBy(x => x.ProductId)
                .Select(g => new DirectCheckoutItemDTO
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(i => i.Quantity)
                })
                .ToList();

            var productIds = normalizedItems.Select(x => x.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId) && p.IsActive)
                .ToListAsync();

            if (products.Count != productIds.Count)
                return BadRequest(new { message = "Some products are invalid or inactive" });

            var productDict = products.ToDictionary(p => p.ProductId);
            var orderDetails = normalizedItems.Select(item => new OrderDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = productDict[item.ProductId].UnitPrice
            }).ToList();

            var totalAmount = orderDetails.Sum(x => x.Quantity * x.UnitPrice);
            if (totalAmount <= 0) return BadRequest(new { message = "Order amount must be greater than 0" });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Address = request.Address,
                    PhoneNumber = request.PhoneNumber,
                    OrderStatusId = 1,
                    PaymentMethodId = request.PaymentMethodId,
                    TotalAmount = totalAmount,
                    OrderDetails = orderDetails
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var createdOrder = await _context.Orders
                    .Include(o => o.OrderStatus)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .FirstAsync(o => o.OrderId == order.OrderId);

                return Ok(MapOrder(createdOrder));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetMyOrders()
        {
            var userId = _accountService.GetCurrentUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized(new { message = "User not found" });

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders.Select(MapOrder));
        }

        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDTO>> GetOrderById(int orderId)
        {
            var userId = _accountService.GetCurrentUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized(new { message = "User not found" });

            var order = await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null) return NotFound(new { message = "Order not found" });

            return Ok(MapOrder(order));
        }

        [HttpGet("payment-methods")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetPaymentMethods()
        {
            var paymentMethods = await _context.PaymentMethod
                .OrderBy(x => x.PaymentMethodId)
                .Select(x => new { x.PaymentMethodId, x.PaymentMethodName })
                .ToListAsync();

            return Ok(paymentMethods);
        }

        private static OrderResponseDTO MapOrder(Order order)
        {
            return new OrderResponseDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                OrderStatusId = order.OrderStatusId,
                OrderStatusName = order.OrderStatus?.OrderStatusName ?? string.Empty,
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod?.PaymentMethodName ?? string.Empty,
                TotalAmount = order.TotalAmount,
                Items = order.OrderDetails.Select(d => new OrderDetailResponseDTO
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName ?? string.Empty,
                    Image = d.Product?.Image ?? string.Empty,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.Quantity * d.UnitPrice
                }).ToList()
            };
        }
    }
}
