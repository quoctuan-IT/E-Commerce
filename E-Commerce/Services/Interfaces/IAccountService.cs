using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels.AccountVM;
using E_Commerce.Areas.Admin.ViewModels.UserVM;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Services.Interfaces
{
    public interface IAccountService
    {
        // Account
        Task<IdentityResult> RegisterAsync(RegisterVM vm);
        Task<SignInResult> LoginAsync(LoginVM vm);
        Task LogoutAsync();
        Task<IdentityResult> UpdateUserProfileAsync(string userId, UpdateProfileVM vm);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordVM vm);


        // Admin
        Task<IdentityResult> CreateUserAsync(UserCreateVM vm);
        Task<IdentityResult> UpdateUserPasswordAsync(string userId, UserPasswordUpdateVM vm);


        // GET
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<AppUser?> GetCurrentUserAsync(System.Security.Claims.ClaimsPrincipal user);
        string? GetCurrentUserId(System.Security.Claims.ClaimsPrincipal user);
        Task<AppUser?> GetUserByIdAsync(string userId);
        Task<AppUser?> GetUserByNameAsync(string userName);
        Task<AppUser?> GetUserByEmailAsync(string userEmail);


        // JWT
        Task<TokenDTO> GenerateJwtTokenAsync(AppUser user);
        Task<AppUser?> ValidateUserAsync(LoginDTO loginDTO);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<UserDTO> GetUserProfileAsync(string userId);
        Task<IdentityResult> UpdateUserProfileAsync(string userId, UserDTO userDTO);
    }
}
