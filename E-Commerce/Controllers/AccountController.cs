using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    public class AccountController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUser.SingleOrDefault(u => u.Phone == vm.Phone);
                if (user != null)
                    ModelState.AddModelError(string.Empty, "Phone exists!");
                else
                {
                    user = new AppUser
                    {
                        Password = vm.Password,
                        FullName = vm.FullName,
                        Phone = vm.Phone,
                        Address = vm.Address
                    };

                    _context.Add(user);
                    _context.SaveChanges();

                    return RedirectToAction("Success");
                }
            }

            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUser.SingleOrDefault(u => u.Phone == vm.Phone && u.Password == vm.Password);

                if (user != null)
                {
                    if (user.IsActive == false)
                        ModelState.AddModelError(string.Empty, "Account locked!");
                    else
                    {
                        var claims = new List<Claim> {
                            new("ID", user.UserId.ToString()),
                            new(ClaimTypes.Name, user.FullName),
                            new(ClaimTypes.Role, user.Role.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);
                    }
                }
                else
                    ModelState.AddModelError(string.Empty, "Login failed!");
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("/");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
