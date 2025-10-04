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
                var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.DienThoai == model.DienThoai);

                if (khachHang != null)
                {
                    ModelState.AddModelError(string.Empty, "PhoneNumber already exists!");
                }
                else
                {
                    try
                    {
                        khachHang = new KhachHang();
                        khachHang.MatKhau = model.MatKhau;
                        khachHang.HoTen = model.HoTen;
                        khachHang.DienThoai = model.DienThoai;
                        khachHang.DiaChi = model.DiaChi;
                        khachHang.HieuLuc = true;

                        _context.Add(khachHang);
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
                var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.DienThoai == model.Phone);

                if (khachHang != null)
                {
                    if (khachHang.HieuLuc == false)
                    {
                        ModelState.AddModelError(string.Empty, "Account has been locked!");
                    }
                    else
                    {
                        if (khachHang.MatKhau != model.Password)
                        {
                            ModelState.AddModelError(string.Empty, "Login failed!");
                        }
                        else
                        {
                            var claims = new List<Claim> {
                                new(ClaimTypes.Name, khachHang.HoTen),
                                new(ClaimTypes.Role, khachHang.VaiTro.ToString()),
                                new("ID", khachHang.MaKh.ToString())
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);

                            if (khachHang.VaiTro == 0)
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
