using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IcreamShopCilent.Pages.Admins
{
  
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DashboardModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7283/");
        }

        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("Role");
            Console.WriteLine($"Dashboard accessed - Role: {role}");
            if (!string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}
