using MyNutritionist.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Nutritionist : ApplicationUser
    {
        [ForeignKey("ApplicationUser")]
        public string AspUserId { get; set; }

        public string Image { get; set; }
        public ICollection<PremiumUser> PremiumUsers { get; set; } = new List<PremiumUser>();
        public Nutritionist(): base() {
        }
        public void SortUsers(ISort strategy)
        {
            PremiumUsers = strategy.Sort(PremiumUsers.ToList());
        }
    }
}
