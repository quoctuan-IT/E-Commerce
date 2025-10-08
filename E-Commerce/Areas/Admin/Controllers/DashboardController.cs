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
        public DashboardController()
        {
        }
        
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
