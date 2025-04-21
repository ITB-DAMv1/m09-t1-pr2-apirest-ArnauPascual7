using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazorPage.Models;

namespace WebRazorPage.Pages
{
    public class AdminRegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminRegisterModel> _logger;

        [BindProperty]
        public RegisterDTO Register { get; set; } = new RegisterDTO();

        public string? ErrorMessage { get; set; }

        public AdminRegisterModel(IHttpClientFactory httpClientFactory, ILogger<AdminRegisterModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiGameJam");
            var response = await client.PostAsJsonAsync("api/auth/admin/registre", Register);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "No estas autoritzat a fer això";
            }
            else
            {
                ErrorMessage = "Error al crear l'usuari";
            }
            return Page();
        }
    }
}
