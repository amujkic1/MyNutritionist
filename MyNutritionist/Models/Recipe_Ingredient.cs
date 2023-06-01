using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Recipe_Ingredient
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }

        [ForeignKey("Ingredient")]
        public int IngredientId { get; set; }
        public Recipe_Ingredient() { }
    }
}
