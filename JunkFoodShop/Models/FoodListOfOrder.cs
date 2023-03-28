namespace JunkFoodShop.Models
{
    public class FoodListOfOrder
    {
        public string FoodName { get; set; }

        public int FoodPrice { get; set; }

        public int Quantity { get; set; }

        public int? UserId { get; set; }
    }
}
