using E_Commerce.Models.ViewModels;

namespace E_Commerce.Services.Interfaces
{
    public interface ICartService
    {
        List<CartItemVM> GetCartItems(ISession session);
        void SaveCartItems(ISession session, List<CartItemVM> cartItems);
        void AddToCart(ISession session, int productId, int quantity = 1);
        void RemoveFromCart(ISession session, int productId);
        void UpdateCartItemQuantity(ISession session, int productId, int quantity);
        void ClearCart(ISession session);
        decimal GetCartTotal(List<CartItemVM> cartItems);
        int GetCartItemCount(List<CartItemVM> cartItems);
    }
}
