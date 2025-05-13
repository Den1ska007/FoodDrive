using FoodDrive.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Orders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return View(await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Dish)
            .Where(o => o.UserId == userId)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmOrder()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart?.Items.Any() != true)
            {
                TempData["Error"] = "Кошик порожній";
                return RedirectToAction("Index", "Cart");
            }

            var customer = await _context.Customers.FindAsync(userId);
            if (customer == null) return Unauthorized();

            var total = cart.Items.Sum(i => i.Dish.Price * i.Quantity);
            if (customer.Balance < total)
            {
                TempData["Error"] = "Недостатньо коштів";
                return RedirectToAction("Checkout");
            }

            foreach (var item in cart.Items)
            {
                item.Dish.Stock -= item.Quantity;
                if (item.Dish.Stock < 0) throw new InvalidOperationException("Недостатній запас");
            }

            customer.Balance -= total;

            var order = new Order
            {
                UserId = userId,
                TotalPrice = total,
                Items = cart.Items.Select(i => new OrderItem
                {
                    DishId = i.DishId,
                    Quantity = i.Quantity
                }).ToList()
            };

            _context.Carts.Remove(cart);
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("Orders");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = ex.Message;
            return RedirectToAction("Checkout");
        }
    }
}