using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    [NotMapped]
    public class Leaderboard
    {
        public List<ApplicationUser> Users { get; set; }
        public Leaderboard() { }
    }
}
