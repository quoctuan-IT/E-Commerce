using System.Diagnostics;
<<<<<<< HEAD

using Microsoft.AspNetCore.Mvc;

using E_Commerce.Models;

namespace E_Commerce.Controllers
{
	public class HomeController(ILogger<HomeController> logger) : Controller
	{

		private readonly ILogger<HomeController> _logger = logger;

		public IActionResult Index()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        public IActionResult AccessDenied()
=======
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
>>>>>>> f08b466 (Setup peoject)
        {
            return View();
        }

<<<<<<< HEAD
=======
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
>>>>>>> f08b466 (Setup peoject)
    }
}
