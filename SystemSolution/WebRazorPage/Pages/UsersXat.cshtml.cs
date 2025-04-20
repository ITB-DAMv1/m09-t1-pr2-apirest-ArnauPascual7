using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebRazorPage.Pages
{
    public class UsersXatModel : PageModel
    {
        public string? Token { get; set; }

        public void OnGet()
        {
            Token = HttpContext.Session.GetString("AuthToken");
        }
    }
}
