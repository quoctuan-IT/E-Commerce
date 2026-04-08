using E_Commerce.Models.ViewModels.AccountVM;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class AccountController(IAccountService accountService) : Controller
    {
        private readonly IAccountService _accountService = accountService;


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return NotFound();

            // Profile
            ProfileVM vm = new()
            {
                UserName = user.UserName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Register
            if ((await _accountService.RegisterAsync(vm)).Succeeded)
            {
                TempData["RegisterSuccess"] = "Registration successful!";

                return RedirectToAction(nameof(Register));
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Login
            if ((await _accountService.LoginAsync(vm)).Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "The UserName or Password is incorrect.");
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfileAsync()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return NotFound();

            UpdateProfileVM vm = new()
            {
                Address = user.Address,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return NotFound();

            // UpdateProfile
            if ((await _accountService.UpdateUserProfileAsync(user.Id, vm)).Succeeded)
            {
                TempData["UpdateProfileSuccess"] = "Update Profile successful!";

                return RedirectToAction(nameof(Profile));
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return NotFound();

            // ChangePassword
            if ((await _accountService.ChangePasswordAsync(user.Id, vm)).Succeeded)
            {
                TempData["ChangePasswordSuccess"] = "Change Password successful!";

                return RedirectToAction(nameof(Profile));
            }

            ModelState.AddModelError(string.Empty, "The current Password is incorrect.");
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AccessDenied() => View();
    }
}
