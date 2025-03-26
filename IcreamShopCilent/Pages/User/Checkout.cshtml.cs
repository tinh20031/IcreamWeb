using IcreamShopApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IcreamShopCilent.Pages.User
{
    public class CheckoutModel : PageModel
    {
        public int CurrentUserId { get; set; }
        public List<CartDTO> CartItems { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public CheckoutModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy UserId từ session
            CurrentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (CurrentUserId == 0)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToPage("/Auth/Login");
            }

            // Lấy token từ session
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToPage("/Auth/Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            {
                // Lấy giỏ hàng
                var cartResponse = await client.GetAsync($"https://localhost:7283/api/CartApi/user/{CurrentUserId}");
                if (cartResponse.IsSuccessStatusCode)
                {
                    var cartJson = await cartResponse.Content.ReadAsStringAsync();
                    CartItems = JsonSerializer.Deserialize<List<CartDTO>>(cartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    CartItems = new List<CartDTO>();
                    var errorContent = await cartResponse.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Không thể tải giỏ hàng. Mã lỗi: {cartResponse.StatusCode}. Chi tiết: {errorContent}";
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi khi tải trang thanh toán: {ex.Message}";
                return Page();
            }
        }
    }
}