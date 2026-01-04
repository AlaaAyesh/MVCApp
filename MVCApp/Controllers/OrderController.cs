using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCApp.Data;
using MVCApp.Models;
using MVCApp.Services;
using MVCApp.ViewModels;
using System.Security.Claims;
using AutoMapper;
using System.Threading.Tasks;

namespace MVCApp.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ApiCategoryService _apiCategoryService;
        private readonly IStripeService _stripeService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApiOrderService _apiOrderService;

        public OrderController(
            ApplicationDbContext context,
            ApiCategoryService apiCategoryService,
            IStripeService stripeService,
            UserManager<IdentityUser> userManager,
            IMapper mapper,
            ApiOrderService apiOrderService)
            : base(apiCategoryService)
        {
            _context = context;
            _apiCategoryService = apiCategoryService;
            _stripeService = stripeService;
            _userManager = userManager;
            _mapper = mapper;
            _apiOrderService = apiOrderService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession(CheckoutViewModel checkoutViewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Checkout", "Cart");
            }

            try
            {
                var sessionId = await _stripeService.CreateCheckoutSessionAsync(checkoutViewModel);
                return Json(new { sessionId });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [Authorize]
        public async Task<IActionResult> Success(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderNumber = GenerateOrderNumber(),
                    TotalAmount = 0, // Assuming total amount is not available in the session
                    TaxAmount = 0, // Assuming tax amount is not available in the session
                    ShippingAmount = 0, // Assuming shipping amount is not available in the session
                    Status = OrderStatus.Pending,
                    StripeSessionId = session_id,
                    ShippingFirstName = "",
                    ShippingLastName = "",
                    ShippingAddress = "",
                    ShippingCity = "",
                    ShippingState = "",
                    ShippingZipCode = "",
                    ShippingCountry = "",
                    ShippingPhone = "",
                    ShippingEmail = ""
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // TODO: Add order items from the new cart logic here.

                var orderViewModel = _mapper.Map<OrderViewModel>(order);
                return View(orderViewModel);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var orders = await _apiOrderService.GetOrdersAsync(GetJwtToken());
            return View(orders);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _apiOrderService.GetOrderAsync(GetJwtToken(), id);
            if (order == null) return NotFound();
            return View(order);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderViewModels = _mapper.Map<List<OrderViewModel>>(orders);
            return View(orderViewModels);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            if (status == OrderStatus.Shipped)
            {
                order.ShippedAt = DateTime.UtcNow;
            }
            else if (status == OrderStatus.Delivered)
            {
                order.DeliveredAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();

            var success = await _stripeService.ProcessWebhookAsync(json, signature);
            return success ? Ok() : BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(object orderRequest)
        {
            var success = await _apiOrderService.CreateOrderAsync(GetJwtToken(), orderRequest);
            if (success)
                return RedirectToAction("MyOrders");
            // Handle error
            return View("Checkout");
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private string GetJwtToken() => HttpContext.Session.GetString("JwtToken");
    }
} 