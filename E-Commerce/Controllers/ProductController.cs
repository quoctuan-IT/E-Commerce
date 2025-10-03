using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using E_Commerce.Models;

namespace E_Commerce.Controllers
{
	public class ProductController(AppDbContext context) : Controller
	{

		private readonly AppDbContext _context = context;

		private async Task LoadCategory()
		{
			var loais = await _context.Loais.ToListAsync();
			ViewData["Loais"] = loais;
		}

		public async Task<IActionResult> Index()
		{
			// List Danh Mục.
			await LoadCategory();

			// List Sản Phẩm.
			var hangHoas = await _context.HangHoas.Include(h => h.MaLoaiNavigation).ToListAsync();

			return View(hangHoas);
		}

		public async Task<IActionResult> FilterByCategory(int idCategory)
		{
			// List Danh Mục.
			await LoadCategory();

			var hangHoas = await _context.HangHoas
				.Include(h => h.MaLoaiNavigation)
				.Where(h => h.MaLoai == idCategory)
				.ToListAsync();

			return View("Index", hangHoas);
		}

		[HttpGet]
		public async Task<IActionResult> Search(string searchKeyword)
		{
			// List Danh Mục.
			await LoadCategory();

			var hangHoas = await _context.HangHoas
				.Include(h => h.MaLoaiNavigation)
				.Where(h => h.TenHh.Contains(searchKeyword))
				.ToListAsync();

			return View("Index", hangHoas);
		}

		public async Task<IActionResult> Details(int idProduct)
		{
			// List Danh Mục.
			await LoadCategory();

			var hangHoas = await _context.HangHoas
				.Include(h => h.MaLoaiNavigation)
				.SingleOrDefaultAsync(h => h.MaHh == idProduct);

			return View(hangHoas);
		}

		[HttpGet]
		public async Task<IActionResult> FilterPrice(string filterOption)
		{
			// Lưu giá trị filterOption để giữ trạng thái dropdown
			ViewData["FilterOption"] = "";

			// List Danh Mục.
			await LoadCategory();

			// List Sản Phẩm.
			var hangHoasQuery = _context.HangHoas.Include(h => h.MaLoaiNavigation).AsQueryable();

			// Áp dụng bộ lọc theo tùy chọn
			if (!string.IsNullOrEmpty(filterOption))
			{
				switch (filterOption)
				{
					case "minToMaxPrice":
						hangHoasQuery = hangHoasQuery.OrderBy(h => h.DonGia);
						break;
					case "maxToMinPrice":
						hangHoasQuery = hangHoasQuery.OrderByDescending(h => h.DonGia);
						break;
				}
			}

			var hangHoas = await hangHoasQuery.ToListAsync();

			ViewData["FilterOption"] = filterOption;

			return View("Index", hangHoas);
		}
	}
}
