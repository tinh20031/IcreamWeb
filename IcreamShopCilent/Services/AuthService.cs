using IcreamShopApi.DTOs;
using System.Text;
using System.Text.Json;

namespace IcreamShopCilent.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7283/");
        }

        public async Task<string> RegisterAsnync(string fullName, string email, string password, string phoneNumber, string address)
        {
            var payload = new { name = fullName, Email = email, Password = password, MobilePhone = phoneNumber, StreetAddress = address };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/register", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<(string Token, string Role)> LoginAsync(string email, string password)
        {
            var payload = new { Email = email, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"API Response: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Bỏ qua phân biệt hoa/thường
                    };
                    var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, options);
                    Console.WriteLine($"Deserialized - Token: {authResponse.Token}, Role: {authResponse.Role}");
                    return (authResponse.Token, authResponse.Role);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Deserialize error: {ex.Message}");
                    return (null, null);
                }
            }
            Console.WriteLine($"Login failed with status: {response.StatusCode}");
            return (null, null);
        }
    }
}
