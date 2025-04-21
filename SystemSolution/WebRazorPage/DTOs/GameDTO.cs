using System.ComponentModel.DataAnnotations;

namespace WebRazorPage.DTOs
{
    public class GameDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string DevTeam { get; set; } = string.Empty;
    }
}
