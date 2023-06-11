using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class RecipeViewModel
    {
        public Recipe input { get; set; } = new Recipe();
        public List<Recipe> recipesToDisplay { get; set; } = new List<Recipe>();

        public RecipeViewModel() { }
    }
}
