using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class User: Person
    {
        public string City { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Points { get; set; }
        public User() { }
    }
}
