using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazorPage.Models;
using System.Net.Http;

namespace WebRazorPage.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public LoginDTO Login { get; set; } = new LoginDTO();
        public string? ErrorMessage { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory, ILogger<LoginModel> logger)
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

            try
            {
                var client = _httpClientFactory.CreateClient("ApiGameJam");
                var response = await client.PostAsJsonAsync("api/Auth/login", Login);
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(token))
                    {
                        HttpContext.Session.SetString("AuthToken", token);
                        _logger.LogInformation("Login susccesfull");
                        return RedirectToPage("/Index");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Login failed: {errorContent}");
                    ErrorMessage = "Email o contrasenya incorrectes.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error en l'inici de sessió";
                _logger.LogError(ex, ErrorMessage);
            }

            return Page();

            /*var client = _httpClientFactory.CreateClient("ApiGameJam");
            var content = new StringContent(JsonSerializer.Serialize(Login), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var token = doc.RootElement.GetProperty("token").GetString();

                HttpContext.Session.SetString("AuthToken", token!);
                _logger.LogInformation("Login susccesfull");

                return RedirectToPage("Index");
            }
            else
            {
                ErrorMessage = "Email o contrasenya incorrectes.";
                return Page();
            }*/
        }
    }
}
