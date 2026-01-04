using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCApp.Repositories;
using MVCApp.Services;
using MVCApp.ViewModels;
using AutoMapper;
using MVCApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MVCApp.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly ApiProductService _apiProductService;
        private readonly ApiCartService _apiCartService;
        private readonly IMapper _mapper;

        public ProductsController(
            ApiProductService apiProductService,
            ApiCartService apiCartService,
            ApiCategoryService categoryService,
            IMapper mapper)
            : base(categoryService)
        {
            _apiProductService = apiProductService;
            _apiCartService = apiCartService;
            _mapper = mapper;
        }

        private string GetJwtToken() => HttpContext.Session.GetString("JwtToken");

        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy, string? sortOrder, int page = 1, int pageSize = 12)
        {
            var products = await _apiProductService.GetProductsAsync(searchTerm, categoryId, minPrice, maxPrice, page, pageSize, sortBy, sortOrder);
            var categoriesVm = await _categoryService.GetCategoriesAsync();
            var categories = categoriesVm.Select(c => new Category {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                imageUrl = c.imageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
            var model = new ProductSearchViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOrder = Enum.TryParse<SortOrder>(sortOrder, out var so) ? so : SortOrder.NameAsc,
                Page = page,
                PageSize = pageSize,
                TotalCount = products.Count, // You may want to get this from the API if paginated
                Products = products,
                Categories = categories
            };
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _apiProductService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Get category ID from the product's category object or use CategoryId
            var categoryId = product.Category?.Id ?? product.CategoryId;
            
            // Get related products from the same category
            var relatedProducts = await _apiProductService.GetRelatedProductsAsync(id, categoryId, 4);
            ViewBag.RelatedProducts = relatedProducts;
            
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var jwtToken = GetJwtToken();
            if (string.IsNullOrEmpty(jwtToken))
            {
                // Not logged in, redirect to login or return error for AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "You must be logged in to add to cart." });
                return RedirectToAction("Login", "Auth");
            }

            var success = await _apiCartService.AddItemAsync(jwtToken, productId, quantity);
            int cartCount = 0;
            if (success)
            {
                var cart = await _apiCartService.GetCartAsync(jwtToken);
                cartCount = cart?.TotalItems ?? 0;
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success, cartCount });
            }

            return RedirectToAction("Index", "Cart");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var products = await _apiProductService.GetAllProductsAsync();
            var productViewModels = _mapper.Map<List<ProductViewModel>>(products);
            return View(productViewModels);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // This would typically load categories for the dropdown
            return View(new ProductViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var success = await _apiProductService.CreateProductAsync(productViewModel, GetJwtToken());
                if (success)
                    return RedirectToAction(nameof(Admin));
                ModelState.AddModelError("", "Failed to create product.");
            }
            return View(productViewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _apiProductService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = _mapper.Map<ProductViewModel>(product);
            return View(productViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel productViewModel)
        {
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var success = await _apiProductService.UpdateProductAsync(id, productViewModel, GetJwtToken());
                if (success)
                    return RedirectToAction(nameof(Admin));
                ModelState.AddModelError("", "Failed to update product.");
            }
            return View(productViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _apiProductService.DeleteProductAsync(id, GetJwtToken());
            if (!success)
                ModelState.AddModelError("", "Failed to delete product.");
            return RedirectToAction(nameof(Admin));
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy, string? sortOrder, int page = 1, int pageSize = 12)
        {
            var products = await _apiProductService.SearchProductsAsync(searchTerm, categoryId, minPrice, maxPrice, page, pageSize, sortBy, sortOrder);
            return PartialView("_ProductGrid", products);
        }
    }
} 