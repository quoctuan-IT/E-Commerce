using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Component
{
    [Authorize]
    public class CartViewComponent(ICartService cartService) : ViewComponent
    {
        private readonly ICartService _cartService = cartService;

        [HttpGet]
        public IViewComponentResult Invoke()
        {
            var cartItems = _cartService.GetCartItems(HttpContext.Session);

            return View(cartItems);
        }
    }
}
