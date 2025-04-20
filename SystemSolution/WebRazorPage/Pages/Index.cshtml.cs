using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazorPage.Models;

namespace WebRazorPage.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IndexModel> _logger;

    public List<Game> Games { get; set; } = new List<Game>();
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }

    public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        Token = HttpContext.Session.GetString("AuthToken");

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
