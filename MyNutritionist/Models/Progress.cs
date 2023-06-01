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

        [ForeignKey("RegisteredUser")]
        public int RId { get; set; }

        [ForeignKey("PremiumUser")]
        public int PUId { get; set; }
        public Progress() { }

    }
}
