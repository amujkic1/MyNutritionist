using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    //[NotMapped]
    public class Leaderboard
    {
        [Key]
        public int LID { get; set; }
        //public List<ApplicationUser> Users { get; set; }
        public Leaderboard() { }
    }
}
