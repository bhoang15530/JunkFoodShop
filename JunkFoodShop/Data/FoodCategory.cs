using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class FoodCategory
{
    public int Categoryid { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CategoryImage { get; set; } = null!;

    public virtual ICollection<Food> Foods { get; } = new List<Food>();
}
