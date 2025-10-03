using E_Commerce.Data;
using E_Commerce.Helpers;
using E_Commerce.Models;
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

        private List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CartKey) ?? [];

        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            return View("~/Views/Cart/Checkout.cshtml", Cart);
        }

        [HttpPost]
        public IActionResult Checkout(Checkout model)
        {
            if (ModelState.IsValid)
            {
                var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == "ID").Value);
                var appUser = new AppUser();

                if (model.DefaultAddress)
                {
                    appUser = _context.AppUser.SingleOrDefault(kh => kh.CustomerId == customerId);
                }

                var order = new Order
                {
                    CustomerId = customerId,
                    FullName = model.HoTen ?? appUser.FullName,
                    Address = model.DiaChi ?? appUser.Address,
                    Phone = model.DienThoai ?? appUser.Phone,
                    OrderDate = DateTime.Now,
                    PaymentMethod = "COD",
                    ShippingMethod = "ShoppeExpress",
                    OrderStatusId = 0,
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
                            Quantity = item.SoLuong,
                            UnitPrice = item.DonGia,
                            ProductId = item.MaHh,
                        });
                    }
                    _context.AddRange(orderDetails);
                    _context.SaveChanges();

                    _context.Database.CommitTransaction();

                    HttpContext.Session.Set<List<CartItem>>(CartKey, []);

                    return RedirectToAction("Success");
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }

            return View("~/Views/Cart/Checkout.cshtml", Cart);
        }

        public IActionResult Success()
        {
            return View("~/Views/Cart/Success.cshtml");
        }
    }
}


