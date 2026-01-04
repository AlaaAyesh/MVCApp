using MVCApp.Models;
using MVCApp.ViewModels;

namespace MVCApp.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetActiveAsync();
        Task<List<Product>> GetFeaturedAsync();
        Task<ProductSearchViewModel> SearchAsync(ProductSearchViewModel searchModel);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 