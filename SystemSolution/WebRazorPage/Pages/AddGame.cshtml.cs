using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Win32;
using WebRazorPage.DTOs;

namespace WebRazorPage.Pages
{
    public class AddGameModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AddGameModel> _logger;

        [BindProperty]
        public GameDTO Game { get; set; } = new GameDTO();
        public string? ErrorMessage { get; set; }

        public AddGameModel(IHttpClientFactory httpClientFactory, ILogger<AddGameModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiGameJam");

            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.PostAsJsonAsync("api/games", Game);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Has de ser administrador per a afegir un joc";
                return Page();
            }
            else
            {
                ErrorMessage = "Error al crear el joc: " + response.StatusCode;
                return Page();
            }
        }
    }
}
