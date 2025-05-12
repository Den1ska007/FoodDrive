using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FoodDrive.Entities;
[Table("cart_items")]
public class CartItem
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("cart_id")]
    [ForeignKey("Cart")]
    public int CartId { get; set; }

    [Column("dish_id")]
    [ForeignKey("Dish")]
    public int DishId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }
    public Cart Cart { get; set; }
    // Навігаційні властивості
    public Dish Dish { get; set; }
}