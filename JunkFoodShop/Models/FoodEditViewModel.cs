namespace JunkFoodShop.Models
{
    public class FoodEditViewModel
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }

        public string FoodImage { get; set; }

        public int FoodPrice { get; set; }

        public int FoodStock { get; set; }

        public string FoodDescription { get; set; }

        public string CategoryName { get; set; }

        public int CategoryId { get; set; }
    }
}
