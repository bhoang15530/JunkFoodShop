namespace JunkFoodShop.Models
{
    public class OrderDetails
    {
        public int OrderId { get; set; }

        public int PhoneReceive { get; set; }

        public string Address { get; set; }

        public int TotalPrice { get; set; }
    }
}
