using System.ComponentModel;

namespace MyNutritionist.Models
{
    public class EnterActivityAndFoodViewModel
    {
        public List<Ingredient> Ingredients { get; set; }
        public Ingredient Breakfast { get; set; }
        [DisplayName("Quantity")]
        public int BreakfastQuantity { get; set; }
        public Ingredient Lunch { get; set; }
        [DisplayName("Quantity")]
        public int LunchQuantity { get; set; }
        public Ingredient  Dinner { get; set; }
        [DisplayName("Quantity")]
        public int DinnerQuantity { get; set; }
        public Ingredient Snacks { get; set; }
        [DisplayName("Quantity")]
        public int SnacksQuantity { get; set; }

        public PhysicalActivity PhysicalActivity { get; set; }

        public EnterActivityAndFoodViewModel() { }
    }
}
