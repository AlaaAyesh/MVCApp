using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using MVCApp.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MVCApp.Services
{
    public class ApiCartService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://acomerce1234.runasp.net/api/cart";

        public ApiCartService(HttpClient httpClient) => _httpClient = httpClient;

        private void SetAuth(string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            Console.WriteLine(jwtToken);
        }

        public async Task<CartViewModel?> GetCartAsync(string jwtToken)
        {
            SetAuth(jwtToken);
            var response = await _httpClient.GetAsync(BaseUrl);
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // { data: { items: [...] } }
                if (root.TryGetProperty("data", out var data) && data.TryGetProperty("items", out var items))
                {
                    var cart = new CartViewModel();
                    cart.Items = new List<CartItemViewModel>();
                    foreach (var item in items.EnumerateArray())
                    {
                        var product = item.GetProperty("product");
                        var cartItem = new CartItemViewModel
                        {
                            CartItemId = item.GetProperty("id").GetInt32(),
                            ProductId = product.GetProperty("id").GetInt32(),
                            ProductName = product.GetProperty("name").GetString() ?? string.Empty,
                            imageUrl = product.GetProperty("imageUrl").GetString(),
                            UnitPrice = product.GetProperty("price").GetDecimal(),
                            Quantity = item.GetProperty("quantity").GetInt32(),
                            StockQuantity = product.GetProperty("stock").GetInt32(),
                        };
                        cart.Items.Add(cartItem);
                    }
                    return cart;
                }

                if (root.TryGetProperty("items", out var items2))
                {
                    var cart = new CartViewModel();
                    cart.Items = JsonSerializer.Deserialize<List<CartItemViewModel>>(items2.GetRawText());
                    return cart;
                }

                if (root.ValueKind == JsonValueKind.Array)
                {
                    var cart = new CartViewModel();
                    cart.Items = JsonSerializer.Deserialize<List<CartItemViewModel>>(json);
                    return cart;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cart deserialization error: {ex.Message}\nJSON: {json}");
            }

            return new CartViewModel();
        }
        
        
        public async Task<bool> AddItemAsync(string jwtToken, int productId, int quantity)
        {
            SetAuth(jwtToken);
            var result = await _httpClient.PostAsJsonAsync($"{BaseUrl}/add", new { productId, quantity });
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(string jwtToken, int cartItemId, int quantity)
        {
            SetAuth(jwtToken);
            var result = await _httpClient.PutAsJsonAsync($"{BaseUrl}/items/{cartItemId}", new { quantity });
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveItemAsync(string jwtToken, int cartItemId)
        {
            SetAuth(jwtToken);
            var result = await _httpClient.DeleteAsync($"{BaseUrl}/items/{cartItemId}");
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> ClearCartAsync(string jwtToken)
        {
            SetAuth(jwtToken);
            var result = await _httpClient.DeleteAsync($"{BaseUrl}/clear");
            return result.IsSuccessStatusCode;
        }
    }
} 