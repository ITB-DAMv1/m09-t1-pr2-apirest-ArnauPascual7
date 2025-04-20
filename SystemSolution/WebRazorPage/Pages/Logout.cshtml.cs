using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebRazorPage.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet() { }
        public IActionResult OnPost()
        {
            HttpContext.Session.Remove("AuthToken");
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}
