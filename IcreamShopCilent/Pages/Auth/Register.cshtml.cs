using Azure;
using IcreamShopApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IcreamShopCilent.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public RegisterDto RegisterDto { get; set; }
        public RegisterModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7283/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ki?m tra ModelState
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", RegisterDto);

            if (response.IsSuccessStatusCode)
            {
                // Handle success, c� th? redirect ??n trang ??ng nh?p ho?c th�ng b�o th�nh c�ng
                return RedirectToPage("/Auth/Login");
            }
            else
            {
                // Handle failure, c� th? l?y chi ti?t l?i t? API ?? th�ng b�o
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Registration failed: {errorMessage}");
                return Page();
            }
        }
    }
}
