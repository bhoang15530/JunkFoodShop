﻿namespace JunkFoodShop.Models
{
    public class OrderManage
    {
        public int OrderId { get; set; }

        public int TotalPrice { get; set; }

        public DateTime DateOrder { get; set; }

        public int StatusId { get; set; }

        public int PaymentId { get; set; }

    }
}