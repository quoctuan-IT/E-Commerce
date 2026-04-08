using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels.AccountVM
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "CurrentPassword is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password must be 6-16 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password must be 6-16 characters.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,16}$", ErrorMessage = "Password must contain letters and numbers.")]
        [Display(Name = "Password")]
        public string NewPassword { get; set; } = null!;
    }
}
