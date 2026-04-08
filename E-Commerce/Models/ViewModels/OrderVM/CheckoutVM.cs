using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels.OrderVM
{
    public class CheckoutVM
    {
        [Required]
        public bool DefaultInformation { get; set; }

        [Required(ErrorMessage = "Payment Method is required.")]
        public int PaymentMethodId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address cannot exceed 50 characters.")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;
    }
}
