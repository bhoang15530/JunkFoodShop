using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class OrderStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
