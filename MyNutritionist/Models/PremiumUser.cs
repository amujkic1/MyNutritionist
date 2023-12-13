using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyNutritionist.Utilities;

namespace MyNutritionist.Models
{
    public class PremiumUser: ApplicationUser
    {
        public string? AccountNumber { get; set; }

        public string? City { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Points { get; set; }
        
        [ForeignKey("ApplicationUser")]
        public string AspUserId { get; set; }

        public PremiumUser() { }

	
	}
}
