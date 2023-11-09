using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Recipe
    {
        [Key]
        public int RID { get; set; }

        [Required]
        [DisplayName("Recipe link")]
        public string RecipeLink { get; set; }
        public Nutritionist Nutritionist { get; set; }

        [DisplayName("Calories")]
        public int TotalCalories { get; set; }

        [Required]
        [DisplayName("Name")]
        public string NameOfRecipe { get; set; }

        public List<DietPlan> DietPlans { get; set; }

        //public List<DietPlanRecipe> DietPlanRecipes { get; set; }
        public Recipe() { }
        
    }

}
