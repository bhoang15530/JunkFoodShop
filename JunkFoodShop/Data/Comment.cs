using System;
using System.Collections.Generic;

namespace JunkFoodShop.Data;

public partial class Comment
{
    public int CommentId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime DateComment { get; set; }

    public int? UserId { get; set; }

    public int? FoodId { get; set; }

    public virtual Food? Food { get; set; }

    public virtual UserAccount? User { get; set; }
}
