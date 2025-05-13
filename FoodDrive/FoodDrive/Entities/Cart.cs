using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodDrive.Entities;
using FoodDrive.Interfaces;

[Table("carts")]
public class Cart
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("user_id")]
    [ForeignKey("Customer")]
    public int UserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Customer Customer { get; set; }
    public decimal Total { get; set; } = 0;
    public List<CartItem> Items { get; set; } = new();
}

