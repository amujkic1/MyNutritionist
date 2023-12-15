using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public class Training
    {
        [Key]
        public int TID { get; set; }

        [DisplayName("Name")]
        public string nameOfTraining { get; set; }

        [DisplayName("Link")]
        public string link { get; set; }
        
        [DisplayName("Image")]
        public string image { get; set; }
        
        [DisplayName("Duration")]
        public int duration { get; set; }
    }
}
