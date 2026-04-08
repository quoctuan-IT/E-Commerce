using E_Commerce.Models.ViewModels.CartVM;

namespace E_Commerce.Services.Interfaces
{
    public interface ICartService
    {
        // Cart
        void SaveCartItems(ISession session, List<CartItemVM> cartItems);
        void AddToCart(ISession session, int productId, int quantity = 1);
        void RemoveFromCart(ISession session, int productId);
        void UpdateCartItemQuantity(ISession session, int productId, int quantity);
        void ClearCart(ISession session);


        // GET
        List<CartItemVM> GetCartItems(ISession session);
        decimal GetCartTotal(List<CartItemVM> cartItems);
        int GetCartItemCount(List<CartItemVM> cartItems);
    }
}
