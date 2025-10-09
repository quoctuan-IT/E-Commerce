using E_Commerce.Helpers;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class OrderController(
        IOrderService orderService,
        IAccountService accountService) : Controller
    {
        private readonly IOrderService _orderService = orderService;

        private readonly IAccountService _accountService = accountService;

        private const string CartKey = "CartSession";

        private List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(CartKey) ?? [];

        [HttpGet]
        public IActionResult Index()
        {
            if (Cart.Count == 0) return RedirectToAction("Index", "Cart");

            return View(Cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM vm)
        {
            if (ModelState.IsValid)
            {
                var userId = _accountService.GetCurrentUserId(User);

                if (userId == null) return Unauthorized();

                var success = await _orderService.CreateOrderAsync(userId, vm, Cart);

                if (success)
                {
                    HttpContext.Session.Set<List<CartItemVM>>(CartKey, []);
                    
                    return RedirectToAction(nameof(Success));
                }
            }

            return View(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _accountService.GetCurrentUserId(User);
            if (userId == null) return Unauthorized();

            var orders = await _orderService.GetUserOrdersAsync(userId);

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null) return NotFound();

            // Check Order - Current User
            var userId = _accountService.GetCurrentUserId(User);
            if (userId is null || userId != order.UserId) return Unauthorized();

            return View("OrderDetail", order);
        }

        public IActionResult Success() => View();
    }
}


