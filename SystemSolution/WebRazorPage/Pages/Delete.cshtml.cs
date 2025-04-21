using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazorPage.Models;

namespace WebRazorPage.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DeleteModel> _logger;

        [BindProperty]
        public Game? Game { get; set; }

        public string? Token { get; set; }

        public string? ErrorMessage { get; set; }

        public DeleteModel(IHttpClientFactory httpClientFactory, ILogger<DeleteModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Token = HttpContext.Session.GetString("AuthToken");

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
            var client = _httpClientFactory.CreateClient("ApiGameJam");

            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.DeleteAsync($"api/games/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Error en eliminar el joc.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Has d'estar identificat per eliminar el joc.";
            }
            else
            {
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
