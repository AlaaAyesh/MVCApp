using Microsoft.AspNetCore.Mvc;
using MVCApp.Services;
using MVCApp.ViewModels;
using System;
using System.Threading.Tasks;

namespace MVCApp.Controllers
{
    public class CartController : BaseController
    {
        private readonly ApiCartService _apiCartService;

        public CartController(
            ApiCartService apiCartService,
            ApiCategoryService categoryService)
            : base(categoryService)
        {
            _apiCartService = apiCartService;
        }

        private string GetJwtToken() => HttpContext.Session.GetString("JwtToken");

        public async Task<IActionResult> Index()
        {
            var jwtToken = GetJwtToken();
            if (string.IsNullOrEmpty(jwtToken))
                return RedirectToAction("Login", "Auth");

            try
            {
                var cart = await _apiCartService.GetCartAsync(jwtToken);
                Console.WriteLine($"GetCartAsync response: {cart}");

                return View(cart);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401"))
            {
                // Token expired or invalid, force re-login
                HttpContext.Session.Remove("JwtToken");
                return RedirectToAction("Login", "Auth");
            }
            catch
            {
                // Handle other errors gracefully
                return View(new CartViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            await _apiCartService.AddItemAsync(GetJwtToken(), productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var success = await _apiCartService.UpdateItemAsync(GetJwtToken(), cartItemId, quantity);
            var cart = await _apiCartService.GetCartAsync(GetJwtToken());
            return Json(new {
                success,
                subtotal = cart.Subtotal,
                taxAmount = cart.TaxAmount,
                shippingAmount = cart.ShippingAmount,
                totalAmount = cart.TotalAmount,
                cartCount = cart.TotalItems
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var success = await _apiCartService.RemoveItemAsync(GetJwtToken(), cartItemId);
            var cart = await _apiCartService.GetCartAsync(GetJwtToken());
            return Json(new {
                success,
                subtotal = cart.Subtotal,
                taxAmount = cart.TaxAmount,
                shippingAmount = cart.ShippingAmount,
                totalAmount = cart.TotalAmount,
                cartCount = cart.TotalItems
            });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var success = await _apiCartService.ClearCartAsync(GetJwtToken());
            var cart = await _apiCartService.GetCartAsync(GetJwtToken());
            return Json(new {
                success,
                subtotal = cart.Subtotal,
                taxAmount = cart.TaxAmount,
                shippingAmount = cart.ShippingAmount,
                totalAmount = cart.TotalAmount,
                cartCount = cart.TotalItems
            });
        }

        public async Task<IActionResult> Checkout()
        {
            var jwtToken = GetJwtToken();
            if (string.IsNullOrEmpty(jwtToken))
                return RedirectToAction("Login", "Auth");

            var cart = await _apiCartService.GetCartAsync(jwtToken);
            if (cart == null || !cart.HasItems)
            {
                return RedirectToAction(nameof(Index));
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                Cart = cart
            };

            return View(checkoutViewModel);
        }

        public IActionResult Payment()
        {
            return View();
        }
    }
} 