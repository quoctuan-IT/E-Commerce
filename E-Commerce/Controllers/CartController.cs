using E_Commerce.Data;
using E_Commerce.Helpers;
using E_Commerce.Models;
using E_Commerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class CartController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        private const string CartKey = "CartSession";

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CartKey) ?? [];

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int idProduct, int quantity = 1)
        {
            var gioHang = Cart;
            // check item exist in CART
            var item = gioHang.SingleOrDefault(p => p.MaHh == idProduct);

            if (item == null)
            {
                var product = _context.Products.SingleOrDefault(p => p.ProductId == idProduct);

                if (product == null)
                {
                    return NotFound();
                }

                item = new CartItem
                {
                    MaHh = product.ProductId,
                    TenHh = product.ProductName,
                    DonGia = product.UnitPrice ?? 0,
                    Hinh = product.Image ?? "img.png",
                    SoLuong = quantity
                };

                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(CartKey, gioHang);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int idProduct)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == idProduct);

            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(CartKey, gioHang);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return RedirectToAction("Checkout", "Order");
        }

        [HttpPost]
        public IActionResult Checkout(Checkout model)
        {
            return RedirectToAction("Checkout", "Order", model);
        }

        public IActionResult Success()
        {
            return RedirectToAction("Success", "Order");
        }
    }
}
