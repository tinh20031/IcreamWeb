using IcreamShopApi.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace IcreamShopCilent.Pages.Admins
{
    public class IceCreamsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<IceCream> IceCreams { get; set; } = new List<IceCream>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public IceCreamsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7283/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JwtToken"));

            // Lấy danh sách IceCream
            var iceCreamResponse = await client.GetAsync("api/IceCreamApi");
            if (iceCreamResponse.IsSuccessStatusCode)
            {
                var iceCreamJson = await iceCreamResponse.Content.ReadAsStringAsync();
                IceCreams = JsonSerializer.Deserialize<List<IceCream>>(iceCreamJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Lấy danh sách Category để điền vào dropdown
            var categoryResponse = await client.GetAsync("api/CategoryApi");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                Categories = JsonSerializer.Deserialize<List<Category>>(categoryJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
    }
}