using E_Commerce.Models;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class UserControllers(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var accounts = await _context.AppUser.ToListAsync();

            return View(accounts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    FullName = vm.FullName,
                    Address = vm.Address
                };

                _context.AppUser.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var account = await _context.AppUser.FindAsync(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UserVM vm)
        {
            var account = await _context.AppUser.FindAsync(id);
            if (account == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                account.FullName = vm.FullName;
                account.Address = vm.Address;

                _context.Update(account);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(account);
        }

        public async Task<IActionResult> Block(int id)
        {
            var account = await _context.AppUser.FindAsync(id);
            if (account == null)
                return NotFound();

            account.IsActive = false;

            _context.Update(account);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
