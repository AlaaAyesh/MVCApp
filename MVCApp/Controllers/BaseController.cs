using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVCApp.Services;
using System.Threading.Tasks;

namespace MVCApp.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApiCategoryService _categoryService;

        public BaseController(ApiCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Fetch categories from API synchronously for now (for demo; use async for production)
            ViewBag.Categories = _categoryService.GetCategoriesAsync().Result;
            base.OnActionExecuting(context);
        }
    }
} 