using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class CaloriesIterator : Iterator
    {
        private int currentRecipe = 0;
        private List<Recipe> recipes { get; set; } = new List<Recipe>();
        public Recipe findNextRecipe()
        {
            if (recipes.Count == 0 || recipes.Count == currentRecipe + 1) return null;

            return recipes[currentRecipe++];
        }

        public Recipe getRecipe()
        {
            if (recipes.Count == 0) return null;
            return recipes[currentRecipe];
        }

        public CaloriesIterator(List<Recipe> recipes)
        {
            this.recipes = recipes;
        }
    }
}
