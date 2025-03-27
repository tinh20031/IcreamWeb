using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IcreamShopCilent.Pages.Admins
{
    public class AdminChatModel : PageModel
    {
        public int CurrentAdminId { get; set; }

        public IActionResult OnGet()
        {
            CurrentAdminId = HttpContext.Session.GetInt32("AdminId") ?? 0;

            // Log giá trị AdminId ra console
            Console.WriteLine($"AdminId từ session: {CurrentAdminId}");

            if (CurrentAdminId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }

    }
}
