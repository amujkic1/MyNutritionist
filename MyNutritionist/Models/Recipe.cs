using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Recipe
    {
        [Key]
        public int RID { get; set; }
        public string RecipeLink { get; set; }
        public Nutritionist Nutritionist { get; set; }
        public int TotalCalories { get; set; }
        [ForeignKey("DietPlan")]
        public int DietPlanID { get; set; }
        public Recipe() { }
    }
}
