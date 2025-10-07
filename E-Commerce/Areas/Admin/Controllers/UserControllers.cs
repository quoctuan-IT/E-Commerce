using E_Commerce.Data;
using E_Commerce.Helpers;
using E_Commerce.Models;
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
        public async Task<IActionResult> Create([Bind("FullName,Password,Address,Phone")] AppUser newAccount)
        {
            newAccount.IsActive = true;

            if (ModelState.IsValid)
            {
                _context.AppUser.Add(newAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(newAccount);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var account = await _context.AppUser.FindAsync(id);
            
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("FullName,Password,Address,Phone")] AppUser updatedAccount)
        {
            var account = await _context.AppUser.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                account.FullName = updatedAccount.FullName;
                account.Password = updatedAccount.Password;
                account.Address = updatedAccount.Address;
                account.Phone = updatedAccount.Phone;

                _context.Update(account);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(updatedAccount);
        }

        public async Task<IActionResult> Block(int id)
        {
            var account = await _context.AppUser.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            account.IsActive = false;

            _context.Update(account);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
