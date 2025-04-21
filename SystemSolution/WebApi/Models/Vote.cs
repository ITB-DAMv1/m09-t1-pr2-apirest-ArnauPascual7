using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Vote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey("GameId")]
        public Game Game { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
