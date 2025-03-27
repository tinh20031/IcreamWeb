using IcreamShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IcreamShopCilent.Pages.User
{
    public class IceCreamDetailModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IceCreamDetailModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IceCream IceCream { get; set; }
        public int CurrentUserId { get; set; } // Lấy UserId từ session hoặc authentication

        public async Task<IActionResult> OnGetAsync(int iceCreamId)
        {
            // Lấy UserId từ session hoặc authentication (tùy vào cách bạn triển khai)
            CurrentUserId = HttpContext.Session.GetInt32("UserId") ?? 0; // Cần triển khai logic xác thực

            // Gọi API để lấy thông tin chi tiết của kem
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7283/api/IceCreamApi/{iceCreamId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                IceCream = JsonSerializer.Deserialize<IceCream>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                IceCream = null;
            }

            return Page();
        }
    }
}