using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JunkFoodShop.Models
{
    public class SignUp
    {
        [Required]
        [RegularExpression(@"^((?!Administrator).)*$", ErrorMessage = "Administrator is not allowed.")]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        // [StringLength(10 ,MinimumLength = 9, ErrorMessage = "The number have at least 9-10 number")]
        public int PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Confirm Password not true")]
        public string ConfirmPassword { get; set; }
    }
}
