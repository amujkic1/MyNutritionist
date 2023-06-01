using System.ComponentModel.DataAnnotations;
namespace MyNutritionist.Models
{
    public class RegisteredUser: Person
    {
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Invalid input")]

        public string City { get; set; }
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Invalid input")]
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Points { get; set; }
        public RegisteredUser() { }
    }
}
