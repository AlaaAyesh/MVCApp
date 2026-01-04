using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using MVCApp.ViewModels;
using System.Text.Json;

namespace MVCApp.Services
{
    public class ApiCategoryService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://acomerce1234.runasp.net/api/products/categories";

        public ApiCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        private async Task<T?> DeserializeFlexibleAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions);
                if (apiResponse.Data != null)
                    return apiResponse.Data;
            }
            catch { }
            try
            {
                var direct = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                if (direct != null)
                    return direct;
            }
            catch { }
            return default;
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            return await DeserializeFlexibleAsync<List<CategoryViewModel>>(response) ?? new List<CategoryViewModel>();
        }
    }
} 