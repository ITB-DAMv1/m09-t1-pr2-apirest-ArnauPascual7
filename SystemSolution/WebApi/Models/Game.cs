using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? DevTeam { get; set; }

        public List<Vote> Votes { get; set; } = new List<Vote>();
    }
}
