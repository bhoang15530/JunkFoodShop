namespace JunkFoodShop.Models
{
    public class FoodManage
    {
        public int FoodId { get; set; }

        public string FoodName { get; set; }
        public string FoodImage { get; set; }
        public int FoodPrice { get; set; }
        public int FoodStock { get; set; }
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
    }
}
