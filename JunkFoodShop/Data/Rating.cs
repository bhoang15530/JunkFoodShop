using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class Rating
{
    public int RatingId { get; set; }

    public int Star { get; set; }

    public int? UserId { get; set; }

    public int? FoodId { get; set; }

    public virtual Food? Food { get; set; }

    public virtual UserAccount? User { get; set; }
}
