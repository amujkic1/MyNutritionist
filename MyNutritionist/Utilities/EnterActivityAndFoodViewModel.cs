namespace MyNutritionist.Models
{
    public class EnterActivityAndFoodViewModel
    {
        public Ingredient Breakfast { get; set; }
        public Ingredient Lunch { get; set; }
        public Ingredient  Dinner { get; set; }
        public Ingredient Snacks { get; set; }

        public PhysicalActivity PhysicalActivity { get; set; }

        public EnterActivityAndFoodViewModel() { }
    }
}
