using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public int TotalPrice { get; set; }

    public DateTime DateOrder { get; set; }

    public int StatusId { get; set; }

    public int PaymentId { get; set; }

    public virtual ICollection<OrderFood> OrderFoods { get; } = new List<OrderFood>();

    public virtual OrderPaymentType? Payment { get; set; }

    public virtual OrderStatus? Status { get; set; }
}
