using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using MVCApp.ViewModels;

namespace MVCApp.Services
{
    public class ApiOrderService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://acomerce1234.runasp.net/api/orders";

        public ApiOrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void SetAuth(string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        public async Task<List<OrderViewModel>> GetOrdersAsync(string jwtToken, int pageNumber = 1, int pageSize = 10)
        {
            SetAuth(jwtToken);
            var url = $"{BaseUrl}?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<OrderViewModel>>>(url);
            return response?.Data ?? new List<OrderViewModel>();
        }

        public async Task<OrderViewModel?> GetOrderAsync(string jwtToken, int id)
        {
            SetAuth(jwtToken);
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<OrderViewModel>>($"{BaseUrl}/{id}");
            return response?.Data;
        }

        public async Task<bool> CreateOrderAsync(string jwtToken, object orderRequest)
        {
            SetAuth(jwtToken);
            var result = await _httpClient.PostAsJsonAsync(BaseUrl, orderRequest);
            return result.IsSuccessStatusCode;
        }
    }
} 