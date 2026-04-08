using Azure.Identity;
using E_Commerce.Areas.Admin.ViewModels.OrderVM;
using E_Commerce.Helpers;
using E_Commerce.Models.Entities;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class OrderController(IOrderService orderService) : Controller
    {
        private readonly IOrderService _orderService = orderService;


        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            List<OrderItemVM> vm = [.. orders.Select(o => new OrderItemVM
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                OrderStatusName = o.OrderStatus?.OrderStatusName ?? "",
                PaymentMethodName = o.PaymentMethod?.PaymentMethodName ?? "",
                TotalAmount = o.TotalAmount,
                TotalItems = o.OrderDetails?.Sum(d => d.Quantity) ?? 0
            })];

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            OrderDetailVM vm = new()
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                UserName = order.AppUser.UserName,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                OrderStatusName = order.OrderStatus?.OrderStatusName ?? "",
                PaymentMethodName = order.PaymentMethod?.PaymentMethodName ?? "",
                TotalAmount = order.TotalAmount,

                OrderDetailItems = [.. order.OrderDetails.Select(d => new OrderDetailItemVM
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName ?? "",
                    Image = d.Product?.Image ?? "",
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity
                })]
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            var orderStatuses = await _orderService.GetAllOrderStatusesAsync();
            if (order == null || orderStatuses == null) return NotFound();

            OrderUpdateVM vm = new()
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderStatusId = order.OrderStatusId,
                OrderStatusName = order.OrderStatus.OrderStatusName,

                OrderStatuses = [.. orderStatuses.Select(o => new OrderStatusItemVM
                {
                    OrderStatusId = o.OrderStatusId,
                    OrderStatusName = o.OrderStatusName
                })]
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(OrderUpdateVM vm)
        {
            await _orderService.UpdateAsync(vm);
            TempData["Success"] = "Update Order successful!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int orderId)
        {
            await _orderService.DeleteOrderAsync(orderId);
            TempData["Success"] = "Delete Order successful!";

            return RedirectToAction(nameof(Index));
        }
    }
}
