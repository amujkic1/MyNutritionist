using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class CaloriesIterator : Iterator
    {
        private int currentRecipe = 0;
        private List<Recipe> recipes { get; set; } = new List<Recipe>();
        public Recipe findNextRecipe()
        {
            if (recipes.Count == 0) return null;

            int calories = recipes[currentRecipe].TotalCalories;
            recipes.Sort((recipe1, recipe2) => recipe2.TotalCalories - recipe1.TotalCalories);
                for(int i = 0; i < recipes.Count; i++)
                {
                    if(i != currentRecipe && recipes[i].TotalCalories < calories)
                {
                    currentRecipe = i;
                    return recipes[i];
                }
                    
                }
            return null;
        }

        public CaloriesIterator(List<Recipe> recipes)
        {
            this.recipes = recipes;
        }
    }
}
