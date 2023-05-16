using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class PremiumUser: User
    {
        public Nutritionist Nutritionist { get; set; }
        public string AccountNumber { get; set; }

        [ForeignKey("Leaderboard")]
        public int LeaderboardId { get; set; }

        [ForeignKey("Nutritionist")]
        public int NutritionistId { get; set; }

        [ForeignKey("DietPlan")]
        public int DietPlanId { get; set; }
        public PremiumUser() { }
    }
}
