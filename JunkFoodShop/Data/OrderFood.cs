using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class OrderFood
{
    public int OrderFoodId { get; set; }

    public int Quantity { get; set; }

    public string Address { get; set; } = null!;

    public int PhoneReceive { get; set; }

    public int? FoodId { get; set; }

    public int? UserId { get; set; }

    public int? OrderId { get; set; }

    public virtual Food? Food { get; set; }

    public virtual Order? Order { get; set; }

    public virtual UserAccount? User { get; set; }
}
