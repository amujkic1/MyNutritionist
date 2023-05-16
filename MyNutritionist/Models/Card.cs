using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class Card
    {
        [Key]
        public int CId { get; set; }
        public int CardNumber { get; set; }
        public PremiumUser Owner { get; set; }
        public Card() { }

    }
}
