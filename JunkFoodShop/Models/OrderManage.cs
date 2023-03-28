namespace JunkFoodShop.Models
{
    public class OrderManage
    {
        public int OrderId { get; set; }

        public int TotalPrice { get; set; }

        public DateTime DateOrder { get; set; }

        public int StatusId { get; set; }

        public int PaymentId { get; set; }
        public string Username { get; set; }
        public int? UserId { get; set; }

    }
}
