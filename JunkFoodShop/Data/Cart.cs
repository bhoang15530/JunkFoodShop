using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class Cart
{
    public int CartId { get; set; }

    public int Quantity { get; set; }

    public string? Address { get; set; }

    public int PhoneReceive { get; set; }

    public int FoodId { get; set; }

    public int UserId { get; set; }

    public virtual Food? Food { get; set; }

    public virtual UserAccount? User { get; set; }
}
