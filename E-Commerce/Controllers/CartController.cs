using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using E_Commerce.Models;
using E_Commerce.ViewModels;
using E_Commerce.Helpers;

namespace E_Commerce.Controllers
{
    public class CartController(AppDbContext context) : Controller
    {

        private readonly AppDbContext _context = context;

        const string CART_KEY = "MYCART";

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? [];

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int idProduct, int quantity = 1)
        {
            var gioHang = Cart;
            // check item exist in CART
            var item = gioHang.SingleOrDefault(p => p.MaHh == idProduct);

            if (item == null)
            {
                var hangHoa = _context.HangHoas.SingleOrDefault(p => p.MaHh == idProduct);

                if (hangHoa == null)
                {
                    return NotFound();
                }

                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? "img.png",
                    SoLuong = quantity
                };

                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(CART_KEY, gioHang);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int idProduct)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == idProduct);

            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(CART_KEY, gioHang);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(Checkout model)
        {
            if (ModelState.IsValid)
            {
                var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == "ID").Value);
                var khachHang = new KhachHang();

                if (model.DefaultAddress)
                {
                    khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "ShoppeExpress",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                _context.Database.BeginTransaction();

                try
                {
                    _context.Add(hoadon);
                    _context.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                        });
                    }
                    _context.AddRange(cthds);
                    _context.SaveChanges();

                    _context.Database.CommitTransaction();

                    HttpContext.Session.Set<List<CartItem>>("MYCART", []);

                    return Redirect("/Cart/Success");
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }

            return View(Cart);
        }

        [Authorize]
        public IActionResult Success()
        {
            return View();
        }
    }
}
