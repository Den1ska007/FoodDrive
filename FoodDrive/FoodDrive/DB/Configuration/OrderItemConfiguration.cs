using FoodDrive.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDrive.DB.Configuration;

public class OrderItemConfiguration :  IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
            .IsRequired();
        
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(oi => oi.Dish)
            .WithMany()
            .HasForeignKey(oi => oi.DishId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ToTable("OrderItems");
    }
}