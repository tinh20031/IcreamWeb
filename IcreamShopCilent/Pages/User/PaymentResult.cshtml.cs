using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IcreamShopCilent.Pages.User
{
    public class PaymentResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Message { get; set; }

        public void OnGet()
        {
            // Message s? ðý?c t? ð?ng gán t? query string
        }
    }
}