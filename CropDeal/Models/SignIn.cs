using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.Models
{
    public class SignIn
    {
        [Required]
        [EmailAddress(ErrorMessage = "Valid Email Required")]
        [RegularExpression(@"[A-Za-z0-9.-_%]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
    }
}