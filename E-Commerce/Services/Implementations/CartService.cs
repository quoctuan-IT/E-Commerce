using E_Commerce.Helpers;
using E_Commerce.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;

namespace E_Commerce.Services.Implementations
{
    public class CartService(AppDbContext context) : ICartService
    {
        private const string CartKey = "CartSession";

        private readonly AppDbContext _context = context;

        public List<CartItemVM> GetCartItems(ISession session)
        {
            return session.Get<List<CartItemVM>>(CartKey) ?? [];
        }

        public void SaveCartItems(ISession session, List<CartItemVM> cartItems)
        {
            session.Set(CartKey, cartItems);
        }

        public void AddToCart(ISession session, int productId, int quantity = 1)
        {
            var cartItems = GetCartItems(session);
            var existingItem = cartItems.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null) existingItem.Quantity += quantity;
            else
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    var newItem = new CartItemVM
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        UnitPrice = product.UnitPrice,
                        Image = product.Image ?? "default.jpg",
                        Quantity = quantity
                    };
                    cartItems.Add(newItem);
                }
            }

            SaveCartItems(session, cartItems);
        }

        public void RemoveFromCart(ISession session, int productId)
        {
            var cartItems = GetCartItems(session);
            var itemToRemove = cartItems.FirstOrDefault(c => c.ProductId == productId);

            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                SaveCartItems(session, cartItems);
            }
        }

        public void UpdateCartItemQuantity(ISession session, int productId, int quantity)
        {
            var cartItems = GetCartItems(session);
            var item = cartItems.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0) cartItems.Remove(item);
                else item.Quantity = quantity;

                SaveCartItems(session, cartItems);
            }
        }

        public void ClearCart(ISession session)
        {
            SaveCartItems(session, []);
        }

        public decimal GetCartTotal(List<CartItemVM> cartItems)
        {
            return (decimal)cartItems.Sum(item => item.UnitPrice * item.Quantity);
        }

        public int GetCartItemCount(List<CartItemVM> cartItems)
        {
            return cartItems.Sum(item => item.Quantity);
        }
    }
}
