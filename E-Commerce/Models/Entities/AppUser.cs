using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Models.Entities;
public partial class AppUser : IdentityUser
{
    public string Address { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<Order> Orders { get; set; } = [];
}
