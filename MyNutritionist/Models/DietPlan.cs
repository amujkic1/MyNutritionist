using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class DietPlan
    {
        [Key]
        public int DPID { get; set; }
        //public PremiumUser User { get; set; }
        public Nutritionist Nutritionist { get; set; }
        public int TotalCalories { get; set; }
        //public Dictionary<DateTime, List<Recipe>> DietList { get; set; }

        [ForeignKey("PremiumUser")]
        public int PremiumUserId { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        
        [ForeignKey("Nutritionist")]
        public int NutritionistID { get; set; }

        public DietPlan() { }
    }
}
