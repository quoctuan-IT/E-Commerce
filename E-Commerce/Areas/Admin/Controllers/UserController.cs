using E_Commerce.Areas.Admin.ViewModels.UserVM;
using E_Commerce.Helpers;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class UserController(IAccountService accountService) : Controller
    {
        private readonly IAccountService _accountService = accountService;


        public async Task<IActionResult> Index()
        {
            var users = await _accountService.GetAllAsync();
            if (users == null) return NotFound();

            List<UserItemVM> vm = [.. users.Select(u => new UserItemVM
            {
                UserId = u!.Id,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
            })];

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Register failed.");

                return View(vm);
            }

            // Register
            if ((await _accountService.CreateUserAsync(vm)).Succeeded)
            {
                TempData["Success"] = "Create New User successful!";

                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string userId)
        {
            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            UserPasswordUpdateVM vm = new()
            {
                UserId = user.Id,
                UserName = user.UserName,
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserPasswordUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            ModelState.AddModelError(string.Empty, "Change Password failed.");

            // ChangePassword
            var user = await _accountService.GetUserByIdAsync(vm.UserId);
            if (user == null) return NotFound();

            if ((await _accountService.UpdateUserPasswordAsync(user.Id, vm)).Succeeded)
            {
                TempData["Success"] = "Change Password successful!";

                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }
    }
}
