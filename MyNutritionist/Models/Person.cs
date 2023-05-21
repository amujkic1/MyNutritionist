using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace MyNutritionist.Models
{
    public class Person
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int PID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
            
        public Person() { }
    }
}
