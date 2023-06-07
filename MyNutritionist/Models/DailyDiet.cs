using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    [NotMapped]
    public class DailyDiet
    {
        public ICollection<KeyValuePair<Ingredient, Double>> FoodIntake { get; set; }
        public DailyDiet() { }

    }
}
