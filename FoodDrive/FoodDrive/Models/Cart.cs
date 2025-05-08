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

    public Cart GetByUserId(int userId)
    {
        return _storage.Load()
            .FirstOrDefault(c => c.UserId == userId);
    }

    public void AddItemToCart(int userId, CartItem item)
    {
        var cart = GetByUserId(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            Add(cart); // Якщо кошика немає, створюємо новий
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.DishId == item.DishId);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity; // Оновлюємо кількість, якщо товар уже є в кошику
        }
        else
        {
            cart.Items.Add(item); // Додаємо новий товар у кошик
        }

        Update(cart); // Оновлюємо кошик
    }

}
