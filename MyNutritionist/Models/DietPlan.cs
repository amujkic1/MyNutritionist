using MyNutritionist.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class DietPlan : ICreateIterator
    {
        [Key]
        public int DPID { get; set; }
        //public Nutritionist Nutritionist { get; set; }
        public int TotalCalories { get; set; }
        public PremiumUser PremiumUser { get; set; } = new PremiumUser();

        public List<Recipe> Recipes { get; set; }
        //public List<DietPlanRecipe> DietPlanRecipes { get; set; } = Enumerable.Repeat(new DietPlanRecipe(), 31).ToList();

        private Iterator iterator { get; set; }

        public Iterator CreateIterator(List<Recipe> recipes)
        {
            if (recipes is null)
            {
                throw new ArgumentNullException(nameof(recipes));
            }
            recipes.Sort((recipe1, recipe2) =>
            {
                return recipe1.TotalCalories - recipe2.TotalCalories;
            });
            return new CaloriesIterator(recipes);
        }
    }
}
