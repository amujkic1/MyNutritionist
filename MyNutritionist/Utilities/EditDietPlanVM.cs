using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class EditDietPlanVM
    {
        public DietPlan DietPlan { get; set; } = new DietPlan();

        //Koristi se samo za ispis recepata
        public List<Recipe> Recipes { get; set; } = new List<Recipe>(31);
    }
}
