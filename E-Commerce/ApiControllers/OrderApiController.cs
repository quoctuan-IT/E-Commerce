using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderApiController(IOrderService orderService, IAccountService accountService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IAccountService _accountService = accountService;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedResultDto<OrderDto>>> GetOrders([FromQuery] OrderFilterDto filter)
        {
            try
            {
                var orders = await _orderService.GetOrdersWithFilterAsync(filter);
                var totalCount = orders.Count();
                var pagedOrders = orders
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(MapToDto);

                var result = new PagedResultDto<OrderDto>
                {
                    Data = pagedOrders,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null) return NotFound(new { message = "Order not found" });

                // Check if user can access this order
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null || (userId != order.UserId && !User.IsInRole("Admin")))
                    return Unauthorized(new { message = "Access denied" });

                return Ok(MapToDto(order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null)
                    return Unauthorized(new { message = "User not found" });

                var success = await _orderService.CreateOrderFromApiAsync(userId, createOrderDto);
                if (!success)
                    return BadRequest(new { message = "Failed to create order" });

                // Get the created order
                var orders = await _orderService.GetUserOrdersAsync(userId);
                var createdOrder = orders.FirstOrDefault();
                if (createdOrder == null)
                    return StatusCode(500, new { message = "Order created but could not be retrieved" });

                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.OrderId }, MapToDto(createdOrder));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Check if user can update this order
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null || (userId != order.UserId && !User.IsInRole("Admin")))
                    return Unauthorized(new { message = "Access denied" });

                var success = await _orderService.UpdateOrderAsync(id, updateOrderDto);
                if (!success)
                    return BadRequest(new { message = "Failed to update order" });

                var updatedOrder = await _orderService.GetOrderByIdAsync(id);
                return Ok(MapToDto(updatedOrder!));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Check if user can delete this order
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null || (userId != order.UserId && !User.IsInRole("Admin")))
                    return Unauthorized(new { message = "Access denied" });

                var success = await _orderService.DeleteOrderAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to delete order" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders(string userId)
        {
            try
            {
                // Check if user can access these orders
                var currentUserId = _accountService.GetCurrentUserId(User);
                if (currentUserId == null || (currentUserId != userId && !User.IsInRole("Admin")))
                    return Unauthorized(new { message = "Access denied" });

                var orders = await _orderService.GetUserOrdersAsync(userId);
                var orderDtos = orders.Select(MapToDto);

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            try
            {
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null)
                    return Unauthorized(new { message = "User not found" });

                var orders = await _orderService.GetUserOrdersAsync(userId);
                var orderDtos = orders.Select(MapToDto);

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("summary")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetOrderSummary([FromQuery] OrderFilterDto filter)
        {
            try
            {
                var orders = await _orderService.GetOrdersWithFilterAsync(filter);
                var summaries = orders.Select(o => new OrderSummaryDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    OrderStatusName = o.OrderStatus.OrderStatusName,
                    TotalAmount = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity),
                    ItemCount = o.OrderDetails.Sum(od => od.Quantity)
                });

                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("status/{statusId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(int statusId)
        {
            try
            {
                var filter = new OrderFilterDto
                {
                    OrderStatusId = statusId,
                    Page = 1,
                    PageSize = 100
                };

                var orders = await _orderService.GetOrdersWithFilterAsync(filter);
                var orderDtos = orders.Select(MapToDto);

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> SearchOrders([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "Search query cannot be empty" });

                var filter = new OrderFilterDto
                {
                    Search = query,
                    Page = 1,
                    PageSize = 100
                };

                var orders = await _orderService.GetOrdersWithFilterAsync(filter);
                var orderDtos = orders.Select(MapToDto);

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                UserName = order.AppUser?.UserName ?? "",
                UserEmail = order.AppUser?.Email ?? "",
                OrderDate = order.OrderDate,
                Address = order.Address,
                Phone = order.Phone,
                PaymentMethod = order.PaymentMethod,
                OrderStatusId = order.OrderStatusId,
                OrderStatusName = order.OrderStatus?.OrderStatusName,
                TotalAmount = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity),
                OrderDetails = [.. order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.ProductName ?? "",
                    ProductImage = od.Product?.Image,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    TotalPrice = od.UnitPrice * od.Quantity
                })]
            };
        }
    }
}
