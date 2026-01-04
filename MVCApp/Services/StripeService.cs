using Microsoft.Extensions.Configuration;
using MVCApp.ViewModels;
using Stripe;
using Stripe.Checkout;

namespace MVCApp.Services
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;
        private readonly string _publishableKey;
        private readonly string _secretKey;
        private readonly string _webhookSecret;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _publishableKey = _configuration["Stripe:PublishableKey"] ?? throw new InvalidOperationException("Stripe PublishableKey not configured");
            _secretKey = _configuration["Stripe:SecretKey"] ?? throw new InvalidOperationException("Stripe SecretKey not configured");
            _webhookSecret = _configuration["Stripe:WebhookSecret"] ?? string.Empty;
            
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<string> CreateCheckoutSessionAsync(CheckoutViewModel checkoutViewModel)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = checkoutViewModel.Cart.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100), // Convert to cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName,
                            Images = !string.IsNullOrEmpty(item.imageUrl) ? new List<string> { item.imageUrl } : null
                        }
                    },
                    Quantity = item.Quantity
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{_configuration["AppUrl"]}/Order/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_configuration["AppUrl"]}/Cart",
                CustomerEmail = checkoutViewModel.Email,
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string> { "US", "CA" }
                },
                Metadata = new Dictionary<string, string>
                {
                    { "customer_email", checkoutViewModel.Email },
                    { "customer_name", $"{checkoutViewModel.FirstName} {checkoutViewModel.LastName}" }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Id;
        }

        public async Task<bool> VerifyPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);
                return paymentIntent.Status == "succeeded";
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ProcessWebhookAsync(string json, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);
                
                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                        // Handle successful checkout
                        await Task.CompletedTask; // Placeholder for future implementation
                        break;
                    case Events.PaymentIntentSucceeded:
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        // Handle successful payment
                        await Task.CompletedTask; // Placeholder for future implementation
                        break;
                    case Events.PaymentIntentPaymentFailed:
                        var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                        // Handle failed payment
                        await Task.CompletedTask; // Placeholder for future implementation
                        break;
                }

                return true;
            }
            catch (StripeException)
            {
                return false;
            }
        }
    }
} 