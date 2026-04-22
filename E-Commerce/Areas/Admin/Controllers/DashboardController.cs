using E_Commerce.Helpers;
using E_Commerce.Models.ViewModels.AccountVM;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class DashboardController(IAccountService accountService) : Controller
    {
        private readonly IAccountService _accountService = accountService;


        public async Task<IActionResult> Index()
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
    }
}
