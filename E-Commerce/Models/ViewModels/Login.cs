using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels
{
    public class Login
    {
        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Phone empty!")]
        [MaxLength(10, ErrorMessage = "Tối đa 10 kí tự!")]
        public required string Phone { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password empty!")]
        [MaxLength(10, ErrorMessage = "Tối đa 10 kí tự!")]
        public required string Password { get; set; }
    }
}
