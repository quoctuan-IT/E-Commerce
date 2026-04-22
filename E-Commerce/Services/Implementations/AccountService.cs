using E_Commerce.Areas.Admin.ViewModels.UserVM;
using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels.AccountVM;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Services.Implementations
{
    public class AccountService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IConfiguration configuration
    ) : IAccountService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;


        // Account
        public async Task<IdentityResult> RegisterAsync(RegisterVM vm)
        {
            var user = new AppUser
            {
                UserName = vm.UserName.Trim(),
                Email = vm.UserName.Trim(),
                PhoneNumber = vm.PhoneNumber,
                Address = vm.Address,
            };

            return await _userManager.CreateAsync(user, vm.Password);
        }

        public async Task<SignInResult> LoginAsync(LoginVM vm)
        {
            var user = await _userManager.FindByNameAsync(vm.UserName);

            if (user != null)
                return await _signInManager.PasswordSignInAsync(
                    user,
                    vm.Password,
                    isPersistent: true,
                    lockoutOnFailure: false);

            return SignInResult.Failed;
        }

        public async Task LogoutAsync()
            => await _signInManager.SignOutAsync();

        public async Task<IdentityResult> UpdateUserProfileAsync(string userId, UpdateProfileVM vm)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            user.PhoneNumber = vm.PhoneNumber;
            user.Address = vm.Address;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordVM vm)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);
        }


        // Admin
        public async Task<IdentityResult> CreateUserAsync(UserCreateVM vm)
        {
            var user = new AppUser
            {
                UserName = vm.UserName,
                PhoneNumber = vm.PhoneNumber,
                Address = vm.Address,
            };

            return await _userManager.CreateAsync(user, vm.Password);
        }

        public async Task<IdentityResult> UpdateUserPasswordAsync(string userId, UserPasswordUpdateVM vm)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);
        }


        // GET
        public async Task<IEnumerable<AppUser>> GetAllAsync()
            => await _userManager.Users.ToListAsync();

        public async Task<AppUser?> GetUserByIdAsync(string userId)
            => await _userManager.FindByIdAsync(userId);

        public async Task<AppUser?> GetUserByEmailAsync(string userEmail)
            => await _userManager.FindByEmailAsync(userEmail);

        public async Task<AppUser?> GetUserByNameAsync(string userName)
            => await _userManager.FindByNameAsync(userName);

        public async Task<AppUser?> GetCurrentUserAsync(System.Security.Claims.ClaimsPrincipal user)
            => await _userManager.GetUserAsync(user);

        public string? GetCurrentUserId(System.Security.Claims.ClaimsPrincipal user)
            => _userManager.GetUserId(user);


        // JWT
        public async Task<TokenDTO> GenerateJwtTokenAsync(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

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

            return new TokenDTO
            {
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresAt = expires
            };
        }

        public async Task<AppUser?> ValidateUserAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDTO.Password)) return user;

            return null;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return string.Empty;

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<UserDTO> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null!;

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Address = user.Address,
            };
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(string userId, UserDTO userDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            user.PhoneNumber = userDTO.PhoneNumber;
            user.Address = userDTO.Address;

            return await _userManager.UpdateAsync(user);
        }
    }
}
