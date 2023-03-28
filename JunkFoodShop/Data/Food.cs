using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class Food
{
    public int FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public string FoodImage { get; set; } = null!;

    public int FoodPrice { get; set; }

    public int FoodStock { get; set; }

    public string FoodDescription { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual ICollection<Cart> Carts { get; } = new List<Cart>();

    public virtual FoodCategory Category { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<OrderFood> OrderFoods { get; } = new List<OrderFood>();

    public virtual ICollection<Rating> Ratings { get; } = new List<Rating>();
}
