using IcreamShopApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IcreamShopCilent.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ProfileModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public AuthResponseDto UserInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Console.WriteLine($"User Profile Page Loaded with ID: {id}");

            if (id == 0)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"https://localhost:7283/api/UserApi/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var userJson = await response.Content.ReadAsStringAsync();
            UserInfo = JsonSerializer.Deserialize<AuthResponseDto>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }


    }
}
