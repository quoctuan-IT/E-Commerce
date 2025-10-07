using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels
{
    public class Register
    {
        [MaxLength(10, ErrorMessage = "Error!")]
        public required string Phone { get; set; }

        [MaxLength(10, ErrorMessage = "Error!")]
        public required string Password { get; set; }

        public required string Name { get; set; }

        public required string Address { get; set; }
    }
}
