using Microsoft.Build.Framework;

namespace JunkFoodShop.Models
{
    public class CreateCategory
    {
        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string CategoryImage { get; set; }
    }
}
