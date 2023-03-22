namespace JunkFoodShop.Models
{
    public class UserOrderList
    {
        public string Fullname { get; set; }
        public int OrderID { get; set; }
        public int TotalPrice { get; set; }

        public DateTime DateOrder { get; set; }

        public int StatusId { get; set; }
        public string PaymentType { get; set; }
    }
}
