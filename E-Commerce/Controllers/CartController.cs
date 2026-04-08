using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class CartController(ICartService cartService) : Controller
    {
        private readonly ICartService _cartService = cartService;


        [HttpGet]
        public IActionResult Index()
        {
            var cartItems = _cartService.GetCartItems(HttpContext.Session);

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            _cartService.AddToCart(HttpContext.Session, productId, quantity);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetCartPartial()
        {
            var cartItems = _cartService.GetCartItems(HttpContext.Session);

            return PartialView("~/Views/Shared/Components/Cart/Default.cshtml", cartItems);
        }

        [HttpGet]
        public IActionResult RemoveCart(int productId)
        {
            _cartService.RemoveFromCart(HttpContext.Session, productId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateCartItemQuantity(HttpContext.Session, productId, quantity);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart(HttpContext.Session);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetCartSummary()
        {
            var cartItems = _cartService.GetCartItems(HttpContext.Session);
            var total = _cartService.GetCartTotal(cartItems);
            var count = _cartService.GetCartItemCount(cartItems);

            return Json(new { total, count });
        }
    }
}
