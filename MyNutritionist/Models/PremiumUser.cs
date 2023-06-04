using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class PremiumUser: Person
    {
        //public Nutritionist Nutritionist { get; set; }
        public string AccountNumber { get; set; }

        [ForeignKey("Leaderboard")]
        public int LeaderboardId { get; set; }

        [ForeignKey("Nutritionist")]
        public int NutritionistId { get; set; }
        public string City { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Points { get; set; }

        public PremiumUser() { }
    }
}
