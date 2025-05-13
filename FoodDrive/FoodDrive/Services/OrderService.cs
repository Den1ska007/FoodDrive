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

    public async Task<OrderResult> PlaceOrder(int cartId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null || cart.Items.Count == 0)
                return new OrderResult { Success = false, Message = "Кошик порожній" };

            var customer = await _context.Customers.FindAsync(cart.UserId);
            if (customer == null)
                return new OrderResult { Success = false, Message = "Клієнта не знайдено" };

            foreach (var item in cart.Items)
            {
                if (item.Dish.Stock < item.Quantity)
                    return new OrderResult { Success = false, Message = $"Недостатньо '{item.Dish.Name}' на складі" };
            }

            var total = cart.Items.Sum(i => i.Dish.Price * i.Quantity);

            if (customer.Balance < total)
                return new OrderResult { Success = false, Message = "Недостатньо коштів" };

            // Оновлення запасів
            foreach (var item in cart.Items)
                item.Dish.Stock -= item.Quantity;

            // Створення замовлення
            var order = new Order
            {
                UserId = customer.Id,
                TotalPrice = total,
                Status = Status.Pending,
                Items = cart.Items.Select(i => new OrderItem
                {
                    DishId = i.DishId,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Оновлення балансу
            customer.Balance -= total;

            // Видалення кошика
            _context.Carts.Remove(cart);

            await _context.Orders.AddAsync(order);
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