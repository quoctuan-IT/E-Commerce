namespace E_Commerce.Models.ViewModels
{
    public class CheckoutVM
    {
        public bool DefaultAddress { get; set; }

        public string FullName { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public string ShippingMethod { get; set; } = null!;
    }
}
