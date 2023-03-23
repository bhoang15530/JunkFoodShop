using Microsoft.Build.Framework;

namespace JunkFoodShop.Models
{
    public class CreateFood
    {
        public string FoodName { get; set; }

        public string FoodImage { get; set; }

        public int FoodPrice { get; set; }

        public int FoodStock { get; set; }

        public string FoodDescription { get; set; }

        public int CategoryId { get; set; }
    }
}
