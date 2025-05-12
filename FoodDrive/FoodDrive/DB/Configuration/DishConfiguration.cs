using FoodDrive.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDrive.DB.Configuration;

public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.HasKey(d => d.Id);
        builder
            .HasMany(d => d.Reviews)
            .WithOne(r => r.Dish)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(d => d.TypeOfDish)
            .HasConversion<string>()  // Зберігати як строку в БД
            .HasMaxLength(24);
    }
}