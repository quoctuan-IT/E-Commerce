using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Services.Implementations
{
    public class AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : IAccountService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        public async Task<IdentityResult> RegisterAsync(RegisterVM vm)
        {
            var user = new AppUser
            {
                UserName = vm.UserName,
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
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<AppUser?> GetCurrentUserAsync(System.Security.Claims.ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public async Task<bool> IsUserActiveAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            return user?.IsActive ?? false;
        }
    }
}
