using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class Leaderboard
    {
        [Key]
        public int LID { get; set; }
        public Leaderboard() { }
    }
}
