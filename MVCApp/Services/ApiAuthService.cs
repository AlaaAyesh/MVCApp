using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using MVCApp.Services;

public class ApiAuthService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://acomerce1234.runasp.net/api/auth";

    public ApiAuthService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<string?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", new { email, password });
        if (!response.IsSuccessStatusCode) return null;

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        return apiResponse?.Data?.Token;
    }

    public async Task<LoginUserResult?> LoginAsyncWithUser(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", new { email, password });
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        if (apiResponse?.Data == null) return null;
        var user = apiResponse.Data.User as System.Text.Json.JsonElement?;
        string? firstName = null;
        if (user != null && user.Value.ValueKind == System.Text.Json.JsonValueKind.Object && user.Value.TryGetProperty("firstName", out var fn))
            firstName = fn.GetString();
        return new LoginUserResult { Token = apiResponse.Data.Token, FirstName = firstName };
    }

    public async Task<bool> RegisterAsync(object registerRequest)
    {
        var result = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", registerRequest);
        return result.IsSuccessStatusCode;
    }
}

public class LoginResponse
{
    public string Token { get; set; }
    public string ExpiresAt { get; set; }
    public object User { get; set; }
}

public class LoginUserResult
{
    public string? Token { get; set; }
    public string? FirstName { get; set; }
} 