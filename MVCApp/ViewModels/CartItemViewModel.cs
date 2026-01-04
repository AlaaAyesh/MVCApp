namespace MVCApp.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? imageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public int StockQuantity { get; set; }
        public int Stock { get => StockQuantity; set => StockQuantity = value; }
        public bool IsAvailable => StockQuantity > 0;
    }
} 