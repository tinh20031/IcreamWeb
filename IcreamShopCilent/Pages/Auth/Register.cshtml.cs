using Azure;
using IcreamShopApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", RegisterDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! Please check your email to verify your account.";
                return RedirectToPage("/Auth/Login");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Registration failed: {errorMessage}");
                return Page();
            }
        }
    }
}
