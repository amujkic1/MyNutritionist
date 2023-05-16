using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class Nutritionist
    {

        [Key]
        public int NID { get; set; }
        public Nutritionist() { }
    }
}
