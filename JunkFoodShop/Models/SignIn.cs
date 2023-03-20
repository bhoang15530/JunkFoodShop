using System.ComponentModel.DataAnnotations;

namespace JunkFoodShop.Models
{
    public class SignIn
    {
        [Required]
        public string UsernameEmail { get; set; }

        [Required]
        public string Password { get; set; }        
    }
}
