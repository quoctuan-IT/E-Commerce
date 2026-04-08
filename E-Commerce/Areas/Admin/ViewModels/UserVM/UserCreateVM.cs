using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Areas.Admin.ViewModels.UserVM
{
    public class UserCreateVM
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address cannot exceed 50 characters.")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password must be 6-16 characters.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,16}$", ErrorMessage = "Password must contain letters and numbers.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;
    }
}
