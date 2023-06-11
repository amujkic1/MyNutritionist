using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class DietPlan
    {
        [Key]
        public int DPID { get; set; }
        //public Nutritionist Nutritionist { get; set; }
        public int TotalCalories { get; set; }
        public PremiumUser PremiumUser { get; set; }
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        public DietPlan() {
            Recipes.Capacity = 28;
        }
    }
}
