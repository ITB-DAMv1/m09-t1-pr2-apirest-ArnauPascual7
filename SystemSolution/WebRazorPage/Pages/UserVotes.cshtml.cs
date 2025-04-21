using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazorPage.Models;

namespace WebRazorPage.Pages
{
    public class UserVotesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserVotesModel> _logger;

        [BindProperty]
        public Dictionary<string, DateTime> GamesVoted { get; set; } = new Dictionary<string, DateTime>();
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }

        public UserVotesModel(IHttpClientFactory httpClientFactory, ILogger<UserVotesModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            Token = HttpContext.Session.GetString("AuthToken");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = _httpClientFactory.CreateClient("ApiGameJam");

            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var votesResponse = await client.GetAsync("api/games/votes");
            if (votesResponse.IsSuccessStatusCode)
            {
                var votesJson = await votesResponse.Content.ReadAsStringAsync();
                var votes = JsonSerializer.Deserialize<List<VoteDTO>>(votesJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                if (votes != null)
                {
                    foreach (var vote in votes)
                    {
                        if (vote.UserId == userId)
                        {
                            var gameResponse = await client.GetAsync($"api/games/{vote.GameId}");
                            if (gameResponse.IsSuccessStatusCode)
                            {
                                var gameJson = await gameResponse.Content.ReadAsStringAsync();
                                var game = JsonSerializer.Deserialize<Game>(gameJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                                if (game != null)
                                {
                                    GamesVoted.Add(game.Title, vote.Date);
                                }
                                else
                                {
                                    ErrorMessage = "No s'ha trobat el joc.";
                                    _logger.LogError(ErrorMessage);
                                }
                            }
                            else
                            {
                                ErrorMessage = $"Error en la càrrega del joc: {votesResponse.StatusCode}";
                                _logger.LogError(ErrorMessage);
                            }
                        }
                    }
                }
                else
                {
                    ErrorMessage = "No s'han trobat vots.";
                    _logger.LogError(ErrorMessage);
                }
            }
            else
            {
                ErrorMessage = $"Error en la càrrega dels vots: {votesResponse.StatusCode}";
                _logger.LogError(ErrorMessage);
            }
        }
    }
}
