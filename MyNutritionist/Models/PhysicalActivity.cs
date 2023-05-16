using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class PhysicalActivity
    {
        [Key] 
        public int PAID { get; set; }
        public ActivityType ActivityType { get; set; }
        public int Duration { get; set; }
        public int NumberOfPoints { get; set; }

        public PhysicalActivity() { }
    }
}
