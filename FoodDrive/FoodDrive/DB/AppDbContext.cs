
using FoodDrive.Entities;
using Microsoft.EntityFrameworkCore;
using FoodDrive.DB.Configuration;

using Microsoft.AspNetCore.Identity;
using System.Reflection;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Customer> Customers => Set<Customer>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasDiscriminator<string>("Discriminator")
        .HasValue<User>("User")
        .HasValue<Admin>("Admin")
        .HasValue<Customer>("Customer");
        modelBuilder.Entity<Cart>()
        .HasMany(c => c.Items)
        .WithOne(i => i.Cart)
        .HasForeignKey(i => i.CartId)
        .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Customer>();
        modelBuilder.Entity<Admin>();
        modelBuilder.ApplyConfiguration(new AdminConfiguration());
        modelBuilder.ApplyConfiguration(new CartConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new DishConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}