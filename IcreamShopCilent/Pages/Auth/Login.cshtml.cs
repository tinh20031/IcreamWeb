using IcreamShopApi.DTOs;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IcreamShopCilent.Services;

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
            return Challenge(new AuthenticationProperties { RedirectUri = "/User/Index" }, FacebookDefaults.AuthenticationScheme);
        }

        // Handler cho việc đăng nhập qua Google
        public IActionResult OnGetGoogleLogin()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/User/Index" }, GoogleDefaults.AuthenticationScheme);
        }

        // Đăng nhập bằng email và mật khẩu
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var authService = new AuthService(_httpClient);
            var (token, role, userId) = await authService.LoginAsync(LoginDto.Email, LoginDto.Password);

            Console.WriteLine($"Login attempt - Token: {token}, Role: {role}, UserId: {userId}");

            if (token != null)
            {
                HttpContext.Session.SetString("JWToken", token);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("Email", LoginDto.Email);
                HttpContext.Session.SetInt32("UserId", userId); // Lưu UserId vào session

                if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                    return RedirectToPage("/Admins/DashboardModel");
                return RedirectToPage("/User/Index");
            }

            ModelState.AddModelError("", "Email or password is incorrect");
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ session
            return RedirectToPage("/Auth/Login");
        }
    }
}