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
        //public List<Ingredient> Ingredients { get; set; }

        [ForeignKey("Nutritionist")]
        public int NutritionistID { get; set; }
        public Recipe() { }
    }
}
