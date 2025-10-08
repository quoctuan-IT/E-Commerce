using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels
{
    public class UserVM
    {
        public string Phone { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Address { get; set; } = null!;
    }
}
