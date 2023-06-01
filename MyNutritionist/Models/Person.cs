using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public abstract class Person
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int PID { get; set; }
        [Required(ErrorMessage = "Invalid input")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "The City field can only contain letters.")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Invalid input")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Invalid input")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Invalid input")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W).*$", ErrorMessage = "The Password field must contain at least one uppercase letter and one non-letter character.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The Password field must be between 8 and 100 characters long.")]
        public string Password { get; set; }
            
        public Person() { }

        /*public Person(int pID, string name, string email, string username, string password)
        {
            PID = pID;
            Name = name;
            Email = email;
            Username = username;
            Password = password;
        }*/
    }
}
