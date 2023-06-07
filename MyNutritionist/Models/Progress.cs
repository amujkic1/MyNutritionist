using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Progress
    {
        [Key]
        public int PRId { get; set; }
        public DateTime Date { get; set; }
        public int BurnedCalories { get; set; }  
        public int ConsumedCalories { get; set; }

        public RegisteredUser RegisteredUser { get; set; }

        public PremiumUser PremiumUser { get; set; }
        public Progress() { }

    }
}
