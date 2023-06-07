using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace MyNutritionist.Models
{
    public class ApplicationUser: IdentityUser
    {
        public override string Id { get; set; }

        [DisplayName("Name")]
        public string? FullName { get; set; }

        [DisplayName("Email")]
        public string? EmailAddress { get; set; }

        [DisplayName("Username")]
        public string? NutriUsername { get; set; }

        [DisplayName("Password")]
        public string? NutriPassword { get; set; }
        public ApplicationUser() { }

    }
}
