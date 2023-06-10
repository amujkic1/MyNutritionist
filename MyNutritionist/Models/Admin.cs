using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{

    public class Admin: ApplicationUser
    {
        [ForeignKey("ApplicationUser")]
        public string AspUserId { get; set; }
        public Admin() { }
    }
}
