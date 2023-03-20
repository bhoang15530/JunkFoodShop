namespace JunkFoodShop.Models
{
    public class OrderStatusSet
    {
        public int OrderId { get; set; }

        public string FoodName { get; set; }

        public int FoodPrice { get; set; }

        public int Quantity { get; set; }

        public int TotalPrice { get; set; }

        public int StatusId { get; set; }

        public int PhoneReceive { get; set; }

        public string Address { get; set; }
    }
}
