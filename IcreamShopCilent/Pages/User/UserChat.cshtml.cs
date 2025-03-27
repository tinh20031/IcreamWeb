using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IcreamShopCilent.Pages.User
{
    public class UserChatModel : PageModel
    {
        public int CurrentUserId { get; set; }

        public IActionResult OnGet()
        {
            // Kiểm tra UserId từ session
            CurrentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            if (CurrentUserId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }
    }
}
