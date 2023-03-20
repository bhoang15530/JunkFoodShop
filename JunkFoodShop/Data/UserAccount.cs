using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class UserAccount
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; } = new List<Cart>();

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<OrderFood> OrderFoods { get; } = new List<OrderFood>();

    public virtual ICollection<Rating> Ratings { get; } = new List<Rating>();
}
