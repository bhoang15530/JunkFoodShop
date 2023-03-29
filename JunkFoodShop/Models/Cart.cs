namespace JunkFoodShop.Models
{
    public class Cart
    {
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal FoodPrice { get; set; }
        public int Quantity { get; set; }
    }
}
