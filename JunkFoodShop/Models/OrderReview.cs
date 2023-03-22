namespace JunkFoodShop.Models
{
    public class OrderReview
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public int OrderId { get; set; }

        public int TotalPrice { get; set; }

        public DateTime DateOrder { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentType { get; set; }
    }
}
