using Microsoft.Build.Framework;

namespace JunkFoodShop.Models
{
    public class CreateFood
    {
        [Required]
        public string FoodName { get; set; }

        [Required]
        public string FoodImage { get; set; }

        [Required]
        public int FoodPrice { get; set; }

        [Required]
        public int FoodStock { get; set; }

        [Required]
        public string FoodDescription { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
