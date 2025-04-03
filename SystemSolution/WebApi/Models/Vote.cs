using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Vote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int GameId { get; set; }
        public string? UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey("GameId")]
        public required Game Game { get; set; }
        [ForeignKey("UserId")]
        public required AppUser User { get; set; }
    }
}
