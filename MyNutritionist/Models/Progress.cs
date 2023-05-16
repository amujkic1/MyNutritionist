using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Progress
    {
        [Key]
        public int PRId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public int BurnedCalories { get; set; }  
        public int ConsumedCalories { get; set; }
        
        [ForeignKey("DailyDiet")]
        public int DailyDietId { get; set; }
        
        public Progress() { }

    }
}
