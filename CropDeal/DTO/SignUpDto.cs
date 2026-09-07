using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CropDeal.Models;

namespace CropDeal.DTO
{
    public class SignUpDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"[A-Za-z0-9.-_%]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password Required")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid Indian mobile number")]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Location should be City name and should not exceed 100 characters")]
        [RegularExpression(@"^[A-Za-z]{3,50}$", ErrorMessage = "Invalid city name")]
        public string Location { get; set; }

        // public UserRole Role { get; set; }
    }
}