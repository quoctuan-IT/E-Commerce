using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels
{
    public class Login
    {
        [MaxLength(10, ErrorMessage = "Error!")]
        public required string Phone { get; set; }

        [MaxLength(10, ErrorMessage = "Error!")]
        public required string Password { get; set; }
    }
}
