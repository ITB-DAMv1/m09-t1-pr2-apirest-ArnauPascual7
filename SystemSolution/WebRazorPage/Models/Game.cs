using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebRazorPage.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? DevTeam { get; set; }
        public int VoteCount { get; set; }
    }
}
