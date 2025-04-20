using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazorPage.Models;

namespace WebRazorPage.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DetailsModel> _logger;

        [BindProperty]
        public Game? Game { get; set; }
        public string? VoteMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public DetailsModel(IHttpClientFactory httpClientFactory, ILogger<DetailsModel> logger)
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
                    Debug.WriteLine("?: Game is null");
                    ErrorMessage = "No s'ha trobat el joc.";
                }
                return Page();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Debug.WriteLine("?: " + response.StatusCode);
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

            // Envia la petició POST per votar a la API
            var response = await client.PostAsync($"api/games/{id}/vote", null);

            if (response.IsSuccessStatusCode)
            {
                VoteMessage = "Vot registrat correctament!";
                // Torna a carregar el joc per actualitzar el recompte de vots
                var gameResponse = await client.GetAsync($"api/games/{id}");
                if (gameResponse.IsSuccessStatusCode)
                {
                    var json = await gameResponse.Content.ReadAsStringAsync();
                    Game = JsonSerializer.Deserialize<Game>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                VoteMessage = !string.IsNullOrWhiteSpace(error) ? error : "No s'ha pogut votar (potser ja has votat aquest joc).";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                VoteMessage = "Has d'estar identificat per votar.";
            }
            else
            {
                VoteMessage = $"Error en votar: {response.StatusCode}";
            }

            return Page();
        }
    }
}
