using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class OrderControllers(IOrderService orderService) : Controller
    {
        private readonly IOrderService _categoryService = orderService;

        public async Task<IActionResult> Index()
        {
            var orders = await _categoryService.GetAllAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var order = await _categoryService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int orderStatusId)
        {
            var order = await _categoryService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            order.OrderStatusId = orderStatusId;
            await _categoryService.UpdateAsync(order);

            return RedirectToAction(nameof(Index));
        }
    }
}
