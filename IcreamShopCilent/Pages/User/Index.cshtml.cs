using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IcreamShopCilent.Pages.User
{
    public class IndexModel : PageModel
    {
        public int CurrentUserId { get; set; }

        public void OnGet()
        {
            // Lấy UserId từ session
            CurrentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
        }
    }
}