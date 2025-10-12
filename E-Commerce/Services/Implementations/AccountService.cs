using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Services.Implementations
{
    public class AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration) : IAccountService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;

        public async Task<IdentityResult> RegisterAsync(RegisterVM vm)
        {
            var user = new AppUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Address = vm.Address,
            };

            return await _userManager.CreateAsync(user, vm.Password);
        }

        public async Task<SignInResult> LoginAsync(LoginVM vm)
        {
            var user = await _userManager.FindByEmailAsync(vm.Email);

            if (user != null && user.IsActive) return await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, lockoutOnFailure: false);

            return SignInResult.Failed;
        }

        public async Task LogoutAsync()
            => await _signInManager.SignOutAsync();

        public async Task<AppUser?> GetUserByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<AppUser?> GetCurrentUserAsync(System.Security.Claims.ClaimsPrincipal user)
            => await _userManager.GetUserAsync(user);

        public async Task<bool> IsUserActiveAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user?.IsActive ?? false;
        }

        public string? GetCurrentUserId(System.Security.Claims.ClaimsPrincipal user)
            => _userManager.GetUserId(user);

        public async Task<TokenDto> GenerateJwtTokenAsync(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add roles if any
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenDto
            {
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresAt = expires
            };
        }

        public async Task<AppUser?> ValidateUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user != null && user.IsActive && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return user;
            }

            return null;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return string.Empty;

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        }

        public async Task<UserDto> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null!;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive,
                CreatedDate = user.LockoutEnd?.DateTime
            };
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(string userId, UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            user.FullName = userDto.FullName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Address = userDto.Address;

            return await _userManager.UpdateAsync(user);
        }
    }
}
