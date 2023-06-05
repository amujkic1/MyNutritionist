using Microsoft.AspNetCore.Identity;

namespace MyNutritionist.Models
{
    public class ApplicationUser: IdentityUser 
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string NutriUsername { get; set; }

        public string NutriPassword { get; set; }
        public ApplicationUser() { }

    }
}
