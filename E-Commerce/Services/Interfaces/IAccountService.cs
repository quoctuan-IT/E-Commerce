using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Services.Interfaces
{
    public interface IAccountService
    {
        Task<SignInResult> LoginAsync(LoginVM vm);
        Task LogoutAsync();
        Task<bool> IsUserActiveAsync(string email);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<AppUser?> GetCurrentUserAsync(System.Security.Claims.ClaimsPrincipal user);
        string? GetCurrentUserId(System.Security.Claims.ClaimsPrincipal user);
        Task<IdentityResult> RegisterAsync(RegisterVM vm);

        // JWT Token methods
        Task<TokenDto> GenerateJwtTokenAsync(AppUser user);
        Task<AppUser?> ValidateUserAsync(LoginDto loginDto);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserDto> GetUserProfileAsync(string userId);
        Task<IdentityResult> UpdateUserProfileAsync(string userId, UserDto userDto);
    }
}
