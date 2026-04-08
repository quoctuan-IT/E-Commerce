using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels.AccountVM
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password must be 6-16 characters.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,16}$", ErrorMessage = "Password must contain letters and numbers.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;
    }
}
