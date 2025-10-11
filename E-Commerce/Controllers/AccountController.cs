using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class AccountController(IAccountService accountService, SignInManager<AppUser> signInManager) : Controller
    {
        private readonly IAccountService _accountService = accountService;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        [Authorize]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterAsync(vm);

                if (result.Succeeded)
                {
                    var user = await _accountService.GetUserByEmailAsync(vm.Email);
                    if (user != null) await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Success");
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }

        public IActionResult Success() => View();

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginAsync(vm);

                if (result.Succeeded) return RedirectToAction("Index", "Home");

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }
    }
}
