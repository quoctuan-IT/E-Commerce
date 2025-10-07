using E_Commerce.Data;
using E_Commerce.Models;
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
        public IActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUser.SingleOrDefault(u => u.Phone == model.DienThoai);

                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "PhoneNumber already exists!");
                }
                else
                {
                    try
                    {
                        user = new AppUser();
                        user.Password = model.MatKhau;
                        user.FullName = model.HoTen;
                        user.Phone = model.DienThoai;
                        user.Address = model.DiaChi;

                        _context.Add(user);
                        _context.SaveChanges();

                        return RedirectToAction("Success");
                    }
                    catch (Exception)
                    {
                        return View();
                    }
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
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUser.SingleOrDefault(u => u.Phone == model.Phone);

                if (user != null)
                {
                    if (user.IsActive == false)
                    {
                        ModelState.AddModelError(string.Empty, "Account has been locked!");
                    }
                    else
                    {
                        if (user.Password != model.Password)
                        {
                            ModelState.AddModelError(string.Empty, "Login failed!");
                        }
                        else
                        {
                            var claims = new List<Claim> {
                                new("ID", user.CustomerId.ToString()),
                                new(ClaimTypes.Name, user.FullName),
                                new(ClaimTypes.Role, user.Role.ToString())
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);

                            if (user.Role == 0)
                            {
                                return Redirect("/Admin/");
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed!");
                }
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
