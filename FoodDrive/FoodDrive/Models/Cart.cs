using System.Collections.Generic;
using FoodDrive.Interfaces;
using FoodDrive.Models;

public class Cart : BaseEntity
{
    public int UserId { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal Total => Items.Sum(i => i.Dish.Price * i.Quantity);
}
public class CartRepository : Repository<Cart>
{
    public CartRepository(IDataStorage<Cart> storage) : base(storage) { }

    public Cart GetByUserId(int userId) =>
        _entities.FirstOrDefault(c => c.UserId == userId);

}
