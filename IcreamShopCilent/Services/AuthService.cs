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
        }

        public async Task<string> RegisterAsnync(string fullName, string email, string password, string phoneNumber, string address)
        {
            var payload = new { name = fullName, Email = email, Password = password, MobilePhone = phoneNumber, StreetAddress = address };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var payload = new { Email = email, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode)
            {
                var token = JsonSerializer.Deserialize<JsonElement>(responseContent).GetProperty("token").GetString();
                return token;
            }
            return null;
        }
    }
}
