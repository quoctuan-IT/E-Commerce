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
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        var categories = await _context.Categories.ToListAsync();
        ViewData["Loais"] = categories;
        
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders.ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> EditOrderStatus(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrderStatus(int id, int orderStatusId)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatusId = orderStatusId;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Orders));
        }

        public async Task<IActionResult> Accounts()
        {
            var accounts = await _context.AppUser.ToListAsync();

            return View(accounts);
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([Bind("FullName,Password,Address,Phone")] AppUser newAccount)
        {
            newAccount.IsActive = true;

            if (ModelState.IsValid)
            {
                _context.AppUser.Add(newAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Accounts));
            }

            return View(newAccount);
        }

        [HttpGet]
        public async Task<IActionResult> EditAccount(int id)
        {
            var account = await _context.AppUser.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> EditAccount(int id, [Bind("FullName,Password,Address,Phone")] AppUser updatedAccount)
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

                return RedirectToAction(nameof(Accounts));
            }

            return View(updatedAccount);
        }

        public async Task<IActionResult> BlockAccount(int id)
        {
            var account = await _context.AppUser.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            account.IsActive = false;

            _context.Update(account);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Accounts));
        }

        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["Loais"] = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([Bind("ProductName,CategoryId,UnitPrice,Image,Description")] Product newProduct, IFormFile uploadedImage)
        {
            if (uploadedImage != null && uploadedImage.Length > 0)
            {
                var imageName = MyUtil.UploadHinh(uploadedImage, "HangHoa");

                newProduct.Image = imageName;
            }
            else
            {
                newProduct.Image = "41Pg1ahql8L._AA300_.jpg";
            }

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Products));
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.ToListAsync();
            ViewData["Loais"] = categories;

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, [Bind("ProductName,CategoryId,UnitPrice,Image,Description")] Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = updatedProduct.ProductName;
            product.CategoryId = updatedProduct.CategoryId;
            product.UnitPrice = updatedProduct.UnitPrice;
            product.Description = updatedProduct.Description;
            product.Image = updatedProduct.Image;

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Products));
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Products));
            }

            return NotFound();
        }
    }
}
