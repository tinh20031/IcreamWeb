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
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7283/api/UserApi/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var userJson = await response.Content.ReadAsStringAsync();
                UserInfo = JsonSerializer.Deserialize<AuthResponseDto>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (UserInfo == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user profile: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
