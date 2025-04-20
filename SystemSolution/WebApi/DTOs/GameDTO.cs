using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        [Required]
        public string DevTeam { get; set; } = string.Empty;
        
        public int VoteCount { get; set; }
    }
}
