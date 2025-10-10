using E_Commerce.Helpers;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class OrderController(IOrderService orderService) : Controller
    {
        private readonly IOrderService _orderService = orderService;

        private const string CartKey = "CartSession";

        private List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(CartKey) ?? [];

        [HttpGet]
        public IActionResult Index()
        {
            if (Cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            return View(Cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM vm)
        {
            if (ModelState.IsValid)
            {
                var claim = HttpContext.User.Claims.SingleOrDefault(u => u.Type == "ID");
                if (claim == null)
                    return Unauthorized();

                var userId = int.Parse(claim.Value);

                var success = await _orderService.CreateOrderAsync(userId, vm, Cart);

                if (success)
                {
                    HttpContext.Session.Set<List<CartItemVM>>(CartKey, []);
                    return RedirectToAction(nameof(Success));
                }
            }

            return View(nameof(Index));
        }

        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var claim = HttpContext.User.Claims.SingleOrDefault(u => u.Type == "ID");
            if (claim == null)
                return Unauthorized();

            var userId = int.Parse(claim.Value);
            var orders = await _orderService.GetUserOrdersAsync(userId);

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null)
                return NotFound();

            // Check if the order belongs to the current user
            var claim = HttpContext.User.Claims.SingleOrDefault(u => u.Type == "ID");
            if (claim is null || int.Parse(claim.Value) != order.UserId)
                return Unauthorized();

            return View("OrderDetail", order);
        }
    }
}


