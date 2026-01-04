using MVCApp.Models;

namespace MVCApp.ViewModels
{
    public enum SortOrder
    {
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc,
        Newest
    }

    public class ProductSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public SortOrder SortOrder { get; set; } = SortOrder.NameAsc;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
} 