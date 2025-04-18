using FoodDrive.Models;
public class CartItem
{
    public int DishId { get; set; }
    public Dish Dish { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Dish?.Price * Quantity ?? 0;
}
