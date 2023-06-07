﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Nutritionist : ApplicationUser
    {
        [ForeignKey("ApplicationUser")]
        public int AspUserId { get; set; }
        public Nutritionist(): base() {
        }
    }
}
