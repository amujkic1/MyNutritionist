namespace MyNutritionist.Models
{
    public class NutritionistsAndUsersViewModel
    {
        public ICollection<PremiumUser> PremiumUsers { get; set; }
        public ICollection<Nutritionist> Nutritionists { get; set;}

        public NutritionistsAndUsersViewModel() { }
    }
}
