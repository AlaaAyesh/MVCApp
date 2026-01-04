using Microsoft.EntityFrameworkCore;
using MVCApp.Data;
using MVCApp.Models;
using MVCApp.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace MVCApp.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ProductRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetActiveAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetFeaturedAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();
        }

        private async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _cache.GetOrCreateAsync("ActiveCategories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await _context.Categories
                    .AsNoTracking()
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            });
        }

        public async Task<ProductSearchViewModel> SearchAsync(ProductSearchViewModel searchModel)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            // Apply search term
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var searchTerm = searchModel.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) || 
                    p.Description!.ToLower().Contains(searchTerm));
            }

            // Apply category filter
            if (searchModel.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchModel.CategoryId.Value);
            }

            // Apply price range filter
            if (searchModel.MinPrice.HasValue)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) >= searchModel.MinPrice.Value);
            }

            if (searchModel.MaxPrice.HasValue)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) <= searchModel.MaxPrice.Value);
            }

            // Apply sorting
            query = searchModel.SortOrder switch
            {
                SortOrder.NameAsc => query.OrderBy(p => p.Name),
                SortOrder.NameDesc => query.OrderByDescending(p => p.Name),
                SortOrder.PriceAsc => query.OrderBy(p => p.SalePrice ?? p.Price),
                SortOrder.PriceDesc => query.OrderByDescending(p => p.SalePrice ?? p.Price),
                SortOrder.Newest => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            // Get total count for pagination
            searchModel.TotalCount = await query.CountAsync();

            // Apply pagination
            var products = await query
                .Skip((searchModel.Page - 1) * searchModel.PageSize)
                .Take(searchModel.PageSize)
                .ToListAsync();

            searchModel.Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                SalePrice = p.SalePrice,
                Stock = p.StockQuantity,
                imageUrl = p.imageUrl,
                SKU = p.SKU,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                CreatedAt = p.CreatedAt,
                CategoryName = p.Category.Name,
                CategoryId = p.CategoryId
            }).ToList();

            // Get categories for filter dropdown
            searchModel.Categories = await GetActiveCategoriesAsync();

            return searchModel;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
    }
} 