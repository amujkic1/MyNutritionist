using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Nutritionist : ApplicationUser
    {
        public Nutritionist(): base() {
        }
    }
}
