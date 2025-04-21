using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebRazorPage.Pages
{
    public class UserVotesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserVotesModel> _logger;

        [BindProperty]
        public List<string> Votes { get; set; } = new List<string>();
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
            var response = await client.GetAsync("api/games");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Games = JsonSerializer.Deserialize<List<Game>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            else
            {
                ErrorMessage = $"Error en la càrrega dels jocs: {response.StatusCode}";
                _logger.LogError(ErrorMessage);
            }
        }
    }
}
