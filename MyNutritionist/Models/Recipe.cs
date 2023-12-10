using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Recipe
    {
        [Key]
        public int RID { get; set; }

        [Required(ErrorMessage = "Recipe link is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        [DisplayName("Recipe link")]
        public string RecipeLink { get; set; }
        public Nutritionist Nutritionist { get; set; }

        [Required]
        [DisplayName("Calories")]
        public int TotalCalories { get; set; }

        [Required(ErrorMessage = "Name of the recipe is required.")]
        [DisplayName("Name")]
        public string NameOfRecipe { get; set; }

        public List<DietPlan> DietPlans { get; set; }

        public Recipe() { }
        
    }

}
