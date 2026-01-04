using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCApp.Models;
using MVCApp.Repositories;
using MVCApp.Services;
using MVCApp.ViewModels;
using AutoMapper;

namespace MVCApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ApiCategoryService _apiCategoryService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductRepository productRepository,
            ApiCategoryService apiCategoryService,
            IMapper mapper) : base(apiCategoryService)
        {
            _logger = logger;
            _productRepository = productRepository;
            _mapper = mapper;
            _apiCategoryService = apiCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productRepository.GetFeaturedAsync();
            var productViewModels = _mapper.Map<List<ProductViewModel>>(featuredProducts);

            // Categories are now handled by BaseController.OnActionExecuting
            return View(productViewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
