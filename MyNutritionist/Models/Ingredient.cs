using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Ingredient
    {
        [Key]
        public int IId { get; set; }
        public string FoodName { get; set; }
        public int Calories { get; set; }
        public double Carbs { get; set; }
        public double Protein { get; set; }
        public double Sugar { get; set; }
        public double Fat { get; set; }
        public double SaturatedFat { get; set; }
        public double VitaminA { get; set; }
        public double VitaminC { get; set; }
        public double Calcium { get; set; }
        public double Iron { get; set; }
        public double Sodium { get; set; }
        
        public Ingredient() { }

    }
}
