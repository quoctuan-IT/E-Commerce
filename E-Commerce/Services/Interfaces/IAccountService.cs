using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM vm);
        Task<SignInResult> LoginAsync(LoginVM vm);
        Task LogoutAsync();
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<AppUser?> GetCurrentUserAsync(System.Security.Claims.ClaimsPrincipal user);
        Task<bool> IsUserActiveAsync(string email);
    }
}
