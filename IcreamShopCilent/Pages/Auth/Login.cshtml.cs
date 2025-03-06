using IcreamShopApi.DTOs;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace IcreamShopCilent.Pages.Auth
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginDto LoginDto { get; set; }
        private readonly HttpClient _httpClient;

        public LoginModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void OnGet()
        {
            // Thực hiện khi người dùng truy cập vào trang login
        }
        // Handler cho việc đăng nhập qua Facebook
        public IActionResult OnGetFacebookLogin()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/Index" }, FacebookDefaults.AuthenticationScheme);
        }

        // Handler cho việc đăng nhập qua Google
        public IActionResult OnGetGoogleLogin()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/Index" }, GoogleDefaults.AuthenticationScheme);
        }

        //login with email and password

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var loginContent = new StringContent(JsonSerializer.Serialize(LoginDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7283/api/auth/login", loginContent);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadAsStringAsync();
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "Email or password is incorrect");
            return Page();
        }
    }
}
