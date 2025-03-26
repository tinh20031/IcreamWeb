using IcreamShopApi.Models;
using IcreamShopApi.DTOs; // Thêm namespace cho CartDTO
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IcreamShopCilent.Pages.User
{
    public class CartModel : PageModel
    {
        public int CurrentUserId { get; set; }
        public List<CartDTO> CartItems { get; set; } // Sử dụng CartDTO thay vì Cart

        private readonly IHttpClientFactory _httpClientFactory;

        public CartModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            // Lấy UserId từ session
            CurrentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (CurrentUserId == 0)
            {
                CartItems = new List<CartDTO>();
                return;
            }

            // Gọi API để lấy danh sách giỏ hàng
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7283/api/CartApi/user/{CurrentUserId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                CartItems = JsonSerializer.Deserialize<List<CartDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                CartItems = new List<CartDTO>();
            }
        }
    }
}