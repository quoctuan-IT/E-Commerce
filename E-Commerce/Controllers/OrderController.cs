using E_Commerce.Helpers;
using E_Commerce.Models.ViewModels.CartVM;
using E_Commerce.Models.ViewModels.OrderVM;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class OrderController(
        IOrderService orderService,
        IAccountService accountService
    ) : Controller
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IAccountService _accountService = accountService;

        private const string CartKey = "CartSession";


        private List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(CartKey) ?? [];

        [HttpGet]
        public async Task<IActionResult> CheckoutAsync()
        {
            if (Cart.Count == 0) return RedirectToAction("Index", "Cart");

            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            // Profile
            CheckoutVM vm = new()
            {
                Address = user.Address,
                PhoneNumber = user.PhoneNumber ?? ""
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = _accountService.GetCurrentUserId(User);
            if (userId == null) return Unauthorized();
            
            // Checkout
            if (await _orderService.CreateOrderAsync(userId, vm, Cart))
            {
                HttpContext.Session.Set<List<CartItemVM>>(CartKey, []);

                TempData["CheckoutSuccess"] = "Checkout Order successful!";
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _accountService.GetCurrentUserId(User);
            if (userId == null) return Unauthorized();

            var orders = await _orderService.GetUserOrdersAsync(userId);

            OrderVM vm = new()
            {
                TotalOrders = orders.Count,

                Orders = [.. orders.Select(o => new OrderItemVM
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    OrderStatusName = o.OrderStatus?.OrderStatusName ?? "",
                    PaymentMethodName = o.PaymentMethod?.PaymentMethodName ?? "",
                    TotalAmount = o.TotalAmount,
                    TotalItems = o.OrderDetails?.Sum(d => d.Quantity) ?? 0
                })]
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null) return NotFound();

            var userId = _accountService.GetCurrentUserId(User);
            if (userId is null || userId != order.UserId) return Unauthorized();

            OrderDetailVM vm = new()
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                OrderStatusName = order.OrderStatus?.OrderStatusName ?? "",
                PaymentMethodName = order.PaymentMethod?.PaymentMethodName ?? "",
                TotalAmount = order.TotalAmount,

                Items = [.. order.OrderDetails.Select(d => new OrderDetailItemVM
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName ?? "",
                    Image = d.Product?.Image ?? "",
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity
                })]
            };

            return View(vm);
        }
    }
}


