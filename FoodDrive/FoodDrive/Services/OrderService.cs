using FoodDrive.Entities;
using FoodDrive.Models;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResult> PlaceOrder(Cart cart)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Перевірка кошика
            var dbCart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            if (dbCart == null || !dbCart.Items.Any())
            {
                return new OrderResult { Success = false, Message = "Кошик порожній" };
            }

            // Перевірка запасів
            foreach (var item in dbCart.Items)
            {
                var dish = await _context.Dishes.FindAsync(item.DishId);
                if (dish == null || dish.Stock < item.Quantity)
                {
                    return new OrderResult
                    {
                        Success = false,
                        Message = $"Недостатньо '{dish?.Name}' на складі"
                    };
                }
            }

            // Перевірка балансу
            var customer = await _context.Users.OfType<Customer>()
                .FirstOrDefaultAsync(c => c.Id == dbCart.UserId);

            var totalPrice = dbCart.Items.Sum(i => i.Dish.Price * i.Quantity);
            if (customer == null || customer.Balance < totalPrice)
            {
                return new OrderResult { Success = false, Message = "Недостатньо коштів" };
            }

            // Оновлення балансу
            customer.Balance -= totalPrice;

            // Оновлення запасів
            foreach (var item in dbCart.Items)
            {
                item.Dish.Stock -= item.Quantity;
            }

            // Створення замовлення
            var order = new Order
            {
                UserId = customer.Id,
                TotalPrice = totalPrice,
                Status = Status.Pending,
                Items = dbCart.Items.Select(i => new OrderItem
                {
                    DishId = i.DishId,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Видалення кошика
            _context.Carts.Remove(dbCart);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new OrderResult { Success = true, Order = order };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new OrderResult { Success = false, Message = ex.Message };
        }
    }
}