using FoodDrive.Models;
public class CartItem : BaseEntity
{
    public int CartId { get; set; }
    public int DishId { get; set; }
    public Dish Dish { get; set; }
    public int Quantity { get; set; }
}