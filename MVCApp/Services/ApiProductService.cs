using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using MVCApp.ViewModels;
using MVCApp.Services;
using System.Text.Json;
using System.Linq;

namespace MVCApp.Services
{
    public class ApiProductService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://acomerce1234.runasp.net/api/products";

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        private async Task<T?> DeserializeFlexibleAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // For product list: look for data.items
                if (root.TryGetProperty("data", out var data))
                {
                    if (data.TryGetProperty("items", out var items))
                    {
                        return JsonSerializer.Deserialize<T>(items.GetRawText(), _jsonOptions);
                    }
                    // For product details: data is the object
                    else if (typeof(T) != typeof(List<ProductViewModel>))
                    {
                        return JsonSerializer.Deserialize<T>(data.GetRawText(), _jsonOptions);
                    }
                }
            }
            catch { }

            // Fallback: try direct deserialization
            try
            {
                var direct = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                if (direct != null)
                    return direct;
            }
            catch { }

            return default;
        }

        public async Task<List<ProductViewModel>> GetProductsAsync(string? searchTerm = null, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? sortOrder = null)
        {
            var url = BaseUrl + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize;
            if (!string.IsNullOrEmpty(searchTerm)) url += "&searchTerm=" + searchTerm;
            if (categoryId.HasValue) url += "&categoryId=" + categoryId.Value;
            if (minPrice.HasValue) url += "&minPrice=" + minPrice.Value;
            if (maxPrice.HasValue) url += "&maxPrice=" + maxPrice.Value;
            if (!string.IsNullOrEmpty(sortBy)) url += "&sortBy=" + sortBy;
            if (!string.IsNullOrEmpty(sortOrder)) url += "&sortOrder=" + sortOrder;


            var response = await _httpClient.GetAsync(url);
            return await DeserializeFlexibleAsync<List<ProductViewModel>>(response) ?? new List<ProductViewModel>();
        }

        public async Task<ProductViewModel?> GetProductAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            return await DeserializeFlexibleAsync<ProductViewModel>(response);
        }

        public async Task<List<ProductViewModel>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            return await DeserializeFlexibleAsync<List<ProductViewModel>>(response) ?? new List<ProductViewModel>();
        }

        public async Task<bool> CreateProductAsync(ProductViewModel product, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var result = await _httpClient.PostAsJsonAsync(BaseUrl, product);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(int id, ProductViewModel product, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var result = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", product);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var result = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return result.IsSuccessStatusCode;
        }

        public async Task<List<ProductViewModel>> SearchProductsAsync(string? searchTerm = null, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? sortOrder = null)
        {
            return await GetProductsAsync(searchTerm, categoryId, minPrice, maxPrice, pageNumber, pageSize, sortBy, sortOrder);
        }

        public async Task<List<ProductViewModel>> GetRelatedProductsAsync(int productId, int categoryId, int limit = 4)
        {
            var url = $"{BaseUrl}?categoryId={categoryId}&pageSize={limit + 1}";
            var products = await DeserializeFlexibleAsync<List<ProductViewModel>>(await _httpClient.GetAsync(url)) ?? new List<ProductViewModel>();
            
            // Filter out the current product and limit results
            return products.Where(p => p.Id != productId).Take(limit).ToList();
        }
    }
} 