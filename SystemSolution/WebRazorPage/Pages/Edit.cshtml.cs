using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazorPage.Models;

namespace WebRazorPage.Pages
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public Game? Game { get; set; }

        [BindProperty]
        public GameDTO NewGame { get; set; } = new GameDTO();

        public string? ErrorMessage { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, ILogger<EditModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiGameJam");
            var response = await client.GetAsync($"api/Games/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Game = JsonSerializer.Deserialize<Game>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (Game == null)
                {
                    ErrorMessage = "No s'ha trobat el joc.";
                }
                return Page();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ErrorMessage = "No s'ha trobat el joc.";
                return Page();
            }
            else
            {
                ErrorMessage = $"Error carregant el joc: {response.StatusCode}";
                _logger.LogError(ErrorMessage);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
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

            var response = await client.PutAsJsonAsync($"api/games/{id}", NewGame);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Has de ser administrador per a editar un joc";
                return Page();
            }
            else
            {
                ErrorMessage = "Error al editar el joc: " + response.StatusCode;
                return Page();
            }
        }
    }
}
