using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class RegisteredUser: ApplicationUser
    {
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Invalid input")]

        public string City { get; set; }
        [Range(6, 100, ErrorMessage = "The age must be between 6 and 100")]
        public int Age { get; set; }
        [Range(0, 300, ErrorMessage = "The weight must be between 0 and 300")]
        public double Weight { get; set; }
        [Range(0, 300, ErrorMessage = "The height must be between 0 and 300")]
        public double Height { get; set; }
        public int Points { get; set; }
        [ForeignKey("ApplicationUser")]
        public int AspUserId { get; set; }
        public RegisteredUser() { }
    }
}
