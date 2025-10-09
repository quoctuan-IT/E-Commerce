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

        public List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(CartKey) ?? [];

        [HttpGet]
        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(c => c.ProductId == productId);

            if (item == null)
            {
                var product = _context.Products.SingleOrDefault(p => p.ProductId == productId);

                if (product != null)
                {
                    item = new CartItemVM
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        UnitPrice = product.UnitPrice,
                        Image = product.Image,
                        Quantity = quantity
                    };

                    cart.Add(item);
                }
            }
            else
                item.Quantity += quantity;

            HttpContext.Session.Set(CartKey, cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCart(int productId)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set(CartKey, cart);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
