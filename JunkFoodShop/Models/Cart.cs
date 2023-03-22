namespace JunkFoodShop.Models
{
    public class Cart
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public int FoodId { get; set; }
        public string Foodname { get; set; }
        public decimal FoodPrice { get; set; }
    }
}
