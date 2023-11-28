namespace MyNutritionist.Models
{
    public class EnterActivityAndFoodViewModel
    {
        public List<Ingredient> Ingredients { get; set; }
        public Ingredient Breakfast { get; set; }
        public int BreakfastQuantity { get; set; }
        public Ingredient Lunch { get; set; }
        public int LunchQuantity { get; set; }
        public Ingredient  Dinner { get; set; }
        public int DinnerQuantity { get; set; }
        public Ingredient Snacks { get; set; }
        public int SnacksQuantity { get; set; }

        public PhysicalActivity PhysicalActivity { get; set; }

        public EnterActivityAndFoodViewModel() { }
    }
}
