using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class NutritionTipsAndQuotes
    {
        [Key]
        public int NTAQId { get; set; }
        public string QuoteText { get; set; }
        public NutritionTipsAndQuotes() { }
    }
}
