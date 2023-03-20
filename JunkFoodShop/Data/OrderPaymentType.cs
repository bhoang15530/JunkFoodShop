using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class OrderPaymentType
{
    public int PaymentId { get; set; }

    public string PaymentType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
