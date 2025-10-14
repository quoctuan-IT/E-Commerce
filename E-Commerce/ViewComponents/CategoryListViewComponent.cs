using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ViewComponents
{
    public class CategoryListViewComponent(ICategoryService service) : ViewComponent
    {
        private readonly ICategoryService _service = service;

        public IViewComponentResult Invoke()
        {
            var categories = _service.GetAllAsync();

            return View(categories);
        }
    }

}
