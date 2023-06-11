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
        public PremiumUser PremiumUser { get; set; }
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        private Iterator iterator { get; set; }

        public DietPlan()
        {
            Recipes.Capacity = 28;
        }

        public Iterator CreateIterator(List<Recipe> recipes)
        {
            if (recipes is null)
            {
                throw new ArgumentNullException(nameof(recipes));
            }
            return new CaloriesIterator(recipes);
        }
    }
}
