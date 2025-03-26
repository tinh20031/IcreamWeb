using IcreamShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserModel = IcreamShopApi.Models.User;
using Microsoft.AspNetCore.Http;
namespace IcreamShopCilent.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ProfileModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public UserModel UserInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7283/api/UserApi/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var userJson = await response.Content.ReadAsStringAsync();
            UserInfo = JsonSerializer.Deserialize<UserModel>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Lấy lại thông tin đầy đủ của user từ API
            var getResponse = await _httpClient.GetAsync($"https://localhost:7283/api/UserApi/{id}");
            if (!getResponse.IsSuccessStatusCode) return NotFound();

            var userJson = await getResponse.Content.ReadAsStringAsync();
            var existingUser = JsonSerializer.Deserialize<UserModel>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (existingUser == null) return NotFound();

            // Gộp thông tin: giữ nguyên các trường không cho sửa
            var userObj = new
            {
                userId = id,
                fullName = UserInfo.FullName,
                email = existingUser.Email,
                passwordHash = existingUser.PasswordHash,
                phoneNumber = UserInfo.PhoneNumber,
                address = UserInfo.Address,
                role = existingUser.Role,
                createdAt = existingUser.CreatedAt
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(userObj), Encoding.UTF8, "application/json");

            var putResponse = await _httpClient.PutAsync($"https://localhost:7283/api/UserApi/{id}", jsonContent);
            if (!putResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile");
                return Page();
            }

            // 🔄 Cập nhật lại tên trong session (header hiển thị đúng sau khi sửa)
            HttpContext.Session.SetString("FullName", UserInfo.FullName);

            TempData["SuccessMessage"] = "Cập nhật thành công";
            return RedirectToPage(new { id });
        }


    }
}
