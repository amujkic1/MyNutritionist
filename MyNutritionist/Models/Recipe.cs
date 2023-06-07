using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Recipe
    {
        [Key]
        public int RID { get; set; }
        public string RecipeLink { get; set; }
        public Nutritionist Nutritionist { get; set; }
        public int TotalCalories { get; set; }

       
        public Recipe() { }
        /*public Recipe()
        {
            // Postavi nutricionista s ID-om 11 kao zadani
            Nutritionist = new Nutritionist { PID = 11 };
        }*/
    }

}
