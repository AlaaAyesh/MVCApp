
namespace MVCApp.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Subtotal => Items.Sum(item => item.TotalPrice);
        public decimal TaxAmount => Subtotal * 0.08m; // 8% tax
        public decimal ShippingAmount => Subtotal > 50 ? 0 : 5.99m; // Free shipping over $50
        public decimal TotalAmount => Subtotal + TaxAmount + ShippingAmount;
        public int TotalItems => Items.Sum(item => item.Quantity);
        public bool HasItems => Items.Any();
    }
} 