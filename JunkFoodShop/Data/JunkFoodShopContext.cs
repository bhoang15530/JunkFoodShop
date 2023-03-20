using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JunkFoodShop.Data;

public partial class JunkFoodShopContext : DbContext
{
    public JunkFoodShopContext()
    {
    }

    public JunkFoodShopContext(DbContextOptions<JunkFoodShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<FoodCategory> FoodCategories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderFood> OrderFoods { get; set; }

    public virtual DbSet<OrderPaymentType> OrderPaymentTypes { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B73F1B178A");

            entity.ToTable("Cart");

            entity.Property(e => e.Address).HasMaxLength(255);

            entity.HasOne(d => d.Food).WithMany(p => p.Carts)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__Cart__FoodId__7E37BEF6");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__UserId__7F2BE32F");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFCAF5043A81");

            entity.ToTable("Comment");

            entity.Property(e => e.Content).HasMaxLength(200);
            entity.Property(e => e.DateComment).HasColumnType("datetime");

            entity.HasOne(d => d.Food).WithMany(p => p.Comments)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__Comment__FoodId__7B5B524B");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comment__UserId__7A672E12");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("PK__Foods__856DB3EB96EC61C6");

            entity.Property(e => e.FoodName).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Foods)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Foods__CategoryI__73BA3083");
        });

        modelBuilder.Entity<FoodCategory>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("PK__FoodCate__19060623FACED0F5");

            entity.ToTable("FoodCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF4F232FB3");

            entity.ToTable("Order");

            entity.Property(e => e.DateOrder).HasColumnType("datetime");

            entity.HasOne(d => d.OrderFood).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderFoodId)
                .HasConstraintName("FK__Order__OrderFood__19DFD96B");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__Order__PaymentId__18EBB532");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Order__StatusId__17F790F9");
        });

        modelBuilder.Entity<OrderFood>(entity =>
        {
            entity.HasKey(e => e.OrderFoodId).HasName("PK__OrderFoo__4EAA48A322619EFD");

            entity.ToTable("OrderFood");

            entity.Property(e => e.Address).HasMaxLength(255);

            entity.HasOne(d => d.Food).WithMany(p => p.OrderFoods)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__OrderFood__FoodI__14270015");

            entity.HasOne(d => d.User).WithMany(p => p.OrderFoods)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__OrderFood__UserI__151B244E");
        });

        modelBuilder.Entity<OrderPaymentType>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__OrderPay__9B556A38A9EB012E");

            entity.ToTable("OrderPaymentType");

            entity.Property(e => e.PaymentType).HasMaxLength(100);
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE20639B757CDD");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Rating__FCCDF87C284552DD");

            entity.ToTable("Rating");

            entity.HasOne(d => d.Food).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__Rating__FoodId__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Rating__UserId__76969D2E");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserAcco__1788CC4CF0EE979D");

            entity.ToTable("UserAccount");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
