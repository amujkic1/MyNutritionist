using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    [NotMapped]
    public class PhysicalActivity
    { 
        public int PAID { get; set; }
        public ActivityType ActivityType { get; set; }
        public int Duration { get; set; }
        public int NumberOfPoints { get; set; }

        public PhysicalActivity() { }
    }
}
