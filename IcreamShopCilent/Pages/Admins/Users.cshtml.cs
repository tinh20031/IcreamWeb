using IcreamShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace IcreamShopCilent.Pages.Admins
{
    public class UsersModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<User> Users { get; set; } = new List<User>();

        public UsersModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7283/"); // Thay bằng URL API của bạn
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JwtToken")); // Giả sử token nằm trong session

            var response = await client.GetAsync("api/UserApi");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
    }
}