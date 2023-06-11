using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Card
    {
        [Key]
        public int CId { get; set; }
        public int CardNumber { get; set; }

        [ForeignKey("PremiumUser")]
        public string PremiumUserId { get; set; }
        public PremiumUser PremiumUser { get; set; }
        public double Balance { get; set; }
        public Card() { }

    }
}
