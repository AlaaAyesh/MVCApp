using MVCApp.ViewModels;

namespace MVCApp.Services
{
    public interface IStripeService
    {
        Task<string> CreateCheckoutSessionAsync(CheckoutViewModel checkoutViewModel);
        Task<bool> VerifyPaymentIntentAsync(string paymentIntentId);
        Task<bool> ProcessWebhookAsync(string json, string signature);
    }
} 