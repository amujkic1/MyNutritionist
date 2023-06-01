using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    [Table("Nutritionists")]
    public class Nutritionist : Person
    {
       
        public Nutritionist(): base() {
        }
    }
}
