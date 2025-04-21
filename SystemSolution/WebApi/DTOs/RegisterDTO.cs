using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "La contrasenya ha contenir mínim 8 caràcters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Les contrasenyes no coincideixen.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
