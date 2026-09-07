using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.Models
{
    public class SignUp
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email Address Required")]
        [RegularExpression(@"[A-Za-z0-9.-_%]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$")]
        public string Email { get; set; }
        

        [Required(ErrorMessage = "Password Required")]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password Required")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid Indian mobile number")]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]{3,50}$", ErrorMessage ="Invalid city name")]
        public string Location { get; set; }

        public UserRole Role { get; set; } 
    }
}