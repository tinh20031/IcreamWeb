using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IcreamShopCilent.Pages.User
{
    public class OrderModel : PageModel
    {
        private readonly ILogger<OrderModel> _logger;

        public int CurrentUserId { get; set; }

        public OrderModel(ILogger<OrderModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Lấy UserId từ session
            CurrentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            _logger.LogInformation("CurrentUserId retrieved from session: {CurrentUserId}", CurrentUserId);


            if (CurrentUserId == 0)
            {
                _logger.LogWarning("UserId not found in session. Redirecting to login.");
                TempData["Error"] = "Vui lòng đăng nhập để xem lịch sử đơn hàng.";
                return RedirectToPage("/Auth/Login");
            }

            // Lấy token từ session
            var token = HttpContext.Session.GetString("JWToken");
            _logger.LogInformation("JWToken retrieved from session: {JWToken}", token);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWToken not found in session. Redirecting to login.");
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }
    }
}
    

