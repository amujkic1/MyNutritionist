using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class DailyDiet
    {
        [Key]
        public int DDID { get; set; }
        public DailyDiet() { }

    }
}
