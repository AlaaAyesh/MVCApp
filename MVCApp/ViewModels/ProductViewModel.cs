namespace MVCApp.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int Stock { get; set; }
        public string? imageUrl { get; set; }
        public string? SKU { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public CategoryViewModel? Category { get; set; }
        
        public bool IsOnSale => SalePrice.HasValue && SalePrice < Price;
        public decimal DisplayPrice => SalePrice ?? Price;
        public int StockQuantity => Stock;
    }
} 