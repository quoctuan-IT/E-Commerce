using E_Commerce.Helpers;
using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class OrderController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

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
        public IActionResult Checkout(CheckoutVM vm)
        {
            if (ModelState.IsValid)
            {
                var claim = HttpContext.User.Claims.SingleOrDefault(u => u.Type == "ID");
                if (claim == null)
                    return Unauthorized();

                var userId = int.Parse(claim.Value);

                // (Optional) - Default address
                var user = new AppUser();
                if (vm.DefaultAddress)
                    user = _context.AppUser.SingleOrDefault(u => u.UserId == userId)
                           ?? throw new InvalidOperationException("User not found");

                var order = new Order
                {
                    UserId = userId,
                    FullName = vm.FullName ?? user.FullName,
                    Address = vm.Address ?? user.Address,
                    Phone = vm.Phone ?? user.Phone,
                    PaymentMethod = vm.PaymentMethod,
                    ShippingMethod = vm.ShippingMethod
                };

                _context.Database.BeginTransaction();

                try
                {
                    _context.Add(order);
                    _context.SaveChanges();

                    var orderDetails = new List<OrderDetail>();
                    foreach (var item in Cart)
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            OrderId = order.OrderId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            ProductId = item.ProductId,
                        });
                    }
                    _context.AddRange(orderDetails);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();

                    HttpContext.Session.Set<List<CartItemVM>>(CartKey, []);

                    return RedirectToAction(nameof(Success));
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }

            return View(nameof(Index));
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}


