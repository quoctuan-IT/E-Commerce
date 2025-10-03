using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using E_Commerce.Models;
using E_Commerce.Helpers;

namespace E_Commerce.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {

        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task LoadCategory()
        {
            var loais = await _context.Loais.ToListAsync();

            ViewData["Loais"] = loais;
        }

        public async Task<IActionResult> Categories()
        {
            var categorys = await _context.Loais.ToListAsync();

            return View(categorys);
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([Bind("TenLoai,MoTa")] Loai newCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Loais.Add(newCategory);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Categories));
            }

            return View(newCategory);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Loais.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, [Bind("TenLoai,MoTa")] Loai updatedCategory)
        {
            var category = await _context.Loais.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                category.TenLoai = updatedCategory.TenLoai;
                category.MoTa = updatedCategory.MoTa;

                _context.Update(category);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Categories));
            }

            return View(updatedCategory);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.HoaDons.ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> EditOrderStatus(int id)
        {
            var order = await _context.HoaDons.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrderStatus(int id, int maTrangThai)
        {
            var order = await _context.HoaDons.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.MaTrangThai = maTrangThai;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Orders));
        }

        public async Task<IActionResult> Accounts()
        {
            var accounts = await _context.KhachHangs.ToListAsync();

            return View(accounts);
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([Bind("HoTen,MatKhau,DiaChi,DienThoai")] KhachHang newAccount)
        {
            newAccount.HieuLuc = true;

            if (ModelState.IsValid)
            {
                _context.KhachHangs.Add(newAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Accounts));
            }

            return View(newAccount);
        }

        [HttpGet]
        public async Task<IActionResult> EditAccount(int id)
        {
            var account = await _context.KhachHangs.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> EditAccount(int id, [Bind("HoTen,MatKhau,DiaChi,DienThoai")] KhachHang updatedAccount)
        {
            var account = await _context.KhachHangs.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                account.HoTen = updatedAccount.HoTen;
                account.MatKhau = updatedAccount.MatKhau;
                account.DiaChi = updatedAccount.DiaChi;
                account.DienThoai = updatedAccount.DienThoai;

                _context.Update(account);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Accounts));
            }

            return View(updatedAccount);
        }

        public async Task<IActionResult> BlockAccount(int id)
        {
            var account = await _context.KhachHangs.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            account.HieuLuc = false;

            _context.Update(account);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Accounts));
        }

        public async Task<IActionResult> Products()
        {
            var products = await _context.HangHoas.Include(h => h.MaLoaiNavigation).ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            await LoadCategory();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([Bind("TenHh,MaLoai,DonGia,Hinh,MoTaDonVi,MoTa,MaNcc")] HangHoa newProduct, IFormFile uploadedImage)
        {
            if (uploadedImage != null && uploadedImage.Length > 0)
            {
                var imageName = MyUtil.UploadHinh(uploadedImage, "HangHoa");

                newProduct.Hinh = imageName;
            }
            else
            {
                newProduct.Hinh = "41Pg1ahql8L._AA300_.jpg";
            }

            _context.HangHoas.Add(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Products));
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.HangHoas.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await LoadCategory();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, [Bind("TenHh,MaLoai,DonGia,Hinh,NgaySx,MoTaDonVi,MoTa")] HangHoa updatedProduct)
        {
            var product = await _context.HangHoas.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.TenHh = updatedProduct.TenHh;
            product.MaLoai = updatedProduct.MaLoai;
            product.DonGia = updatedProduct.DonGia;
            product.MoTa = updatedProduct.MoTa;
            product.Hinh = updatedProduct.Hinh;

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Products));
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var product = await _context.HangHoas.FindAsync(id);

            if (product != null)
            {
                _context.HangHoas.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Products));
            }

            return NotFound();
        }
    }
}
