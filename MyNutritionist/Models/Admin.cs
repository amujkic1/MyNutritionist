using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class Admin: Person
    {
        [Key]
        public int AdId { get; set; }
        public Admin() { }
    }
}
