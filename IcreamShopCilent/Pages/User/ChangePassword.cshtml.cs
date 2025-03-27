using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;

namespace IcreamShopCilent.Pages.User
{
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        [Required]
        public string CurrentPassword { get; set; }

        [BindProperty]
        [Required]
       
        public string NewPassword { get; set; }

        [BindProperty]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userId = HttpContext.Session.GetInt32("UserId");
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Chưa đăng nhập.";
                return RedirectToPage("/Auth/Login");
            }

            var client = new HttpClient();
            var baseUrl = "https://localhost:7283/api/UserApi/";

            // Bước 1: Lấy thông tin user
            var getResponse = await client.GetAsync(baseUrl + userId);
            if (!getResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return Page();
            }

            var json = await getResponse.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Bước 2: Kiểm tra mật khẩu hiện tại trước khi thay đổi (nếu cần)
            if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, user.PasswordHash))
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng.";
                return Page();
            }

            // Gán mật khẩu mới đã mã hoá
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            // Gửi PUT
            var putContent = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            var putResponse = await client.PutAsync(baseUrl + userId, putContent);

            if (putResponse.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToPage("/User/ChangePassword");
            }

            // Nếu lỗi thì log ra
            var responseContent = await putResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Lỗi từ API: " + responseContent);
            TempData["ErrorMessage"] = "Cập nhật thất bại: " + responseContent;
            return Page();
        }

        public class UserDto
        {
            public int UserId { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public string PasswordHash { get; set; }
            public string Role { get; set; }
            public DateTime Created { get; set; }
        }
    }
}
