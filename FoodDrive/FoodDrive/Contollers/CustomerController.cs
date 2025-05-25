using FoodDrive.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using FoodDrive.Models.ViewModels;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(
        AppDbContext context,
        ILogger<CustomerController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IActionResult> Index()
    {
        var dishes = await _context.Dishes
        .Include(d => d.Reviews)
        .ToListAsync();
        return View(dishes);
    }
    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                TempData["Error"] = "Замовлення не знайдено";
                return RedirectToAction(nameof(Orders));
            }

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні деталей замовлення");
            TempData["Error"] = "Сталася помилка";
            return RedirectToAction(nameof(Orders));
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                TempData["Error"] = "Замовлення не знайдено";
                return RedirectToAction("Orders");
            }

            // Перевірка, чи можна скасувати
            if (order.Status != Status.Pending && order.Status != Status.InProgress)
            {
                TempData["Error"] = "Неможливо скасувати замовлення з поточним статусом";
                return RedirectToAction("OrderDetails", new { id });
            }

            // Повернення коштів
            var customer = await _context.Customers.FindAsync(order.UserId);
            if (customer != null)
            {
                customer.Balance += order.TotalPrice;
            }

            // Оновлення статусу
            order.Status = Status.Cancelled;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Замовлення успішно скасовано!";
            return RedirectToAction("OrderDetails", new { id });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Помилка при скасуванні замовлення");
            TempData["Error"] = "Сталася помилка при скасуванні";
            return RedirectToAction("OrderDetails", new { id });
        }
    }
    [HttpGet]
    public async Task<IActionResult> DishDetails(int id)
    {
        var dish = await _context.Dishes
        .Include(d => d.Reviews)
            .ThenInclude(r => r.User)
        .FirstOrDefaultAsync(d => d.Id == id);

        if (dish == null)
        {
            return NotFound();
        }

        return View(dish);
    }
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();

        var cart = await GetUserCart(userId);
        if (cart?.Items?.Any() != true) return RedirectToCart();

        var customer = await GetCustomer(userId);

        var model = new CheckoutViewModel
        {
            Items = cart.Items.Select(i => new CartItemViewModel
            {
                Dish = i.Dish,
                Quantity = i.Quantity
            }).ToList(),
            TotalPrice = CalculateTotal(cart),
            CustomerName = customer.Name,
            CustomerAddress = customer.Address
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmOrder()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userId = GetCurrentUserId();
            var cart = await GetUserCart(userId);

            if (!ValidateCart(cart, out var errorMessage))
            {
                TempData["Error"] = errorMessage;
                return RedirectToCart();
            }

            var customer = await GetCustomer(userId);
            var total = CalculateTotal(cart);

            if (!CheckBalance(customer, total))
            {
                TempData["Error"] = "Недостатньо коштів на рахунку";
                return RedirectToCart();
            }

            await ProcessOrder(cart, customer, total);
            await transaction.CommitAsync();

            TempData["Success"] = "Замовлення успішно оформлено!";
            return RedirectToAction(nameof(Orders));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Помилка при оформленні замовлення");
            TempData["Error"] = "Сталася критична помилка. Спробуйте пізніше";
            return RedirectToCart();
        }
    }

    public async Task<IActionResult> Orders()
    {
        try
        {
            var userId = GetCurrentUserId();
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Dish)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();

            return View(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні замовлень");
            TempData["Error"] = "Сталася помилка при завантаженні замовлень";
            return RedirectToAction("Index", "Home");
        }
    }

    
    #region Private Helpers

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    private async Task<Cart> GetUserCart(int userId) =>
        await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Dish)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    private bool ValidateCart(Cart cart, out string errorMessage)
    {
        errorMessage = null;

        if (cart == null || !cart.Items.Any())
        {
            errorMessage = "Кошик порожній";
            return false;
        }

        foreach (var item in cart.Items)
        {
            if (item.Dish.Stock < item.Quantity)
            {
                errorMessage = $"Недостатньо '{item.Dish.Name}' на складі";
                return false;
            }
        }

        return true;
    }

    private async Task<Customer> GetCustomer(int userId) =>
        await _context.Customers.FindAsync(userId)
        ?? throw new InvalidOperationException("Користувача не знайдено");

    private decimal CalculateTotal(Cart cart) =>
        cart.Items.Sum(i => i.Dish.Price * i.Quantity);

    private bool CheckBalance(Customer customer, decimal total) =>
        customer.Balance >= total;

    private async Task ProcessOrder(Cart cart, Customer customer, decimal total)
    {
        foreach (var item in cart.Items)
        {
            item.Dish.Stock -= item.Quantity;
        }
        
        var order = new Order
        {
            UserId = customer.Id,
            TotalPrice = total,
            Status = Status.Pending,
            OrderDate = DateTime.UtcNow,
            Items = cart.Items.Select(i => new OrderItem
            {
                DishId = i.DishId,
                Quantity = i.Quantity
            }).ToList()
        };

        customer.Balance -= total;

        _context.Carts.Remove(cart);

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }
    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> AddReview(CreateReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Повернення на сторінку страви з помилками
            return RedirectToAction("DishDetails", new { id = model.DishId });
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var dish = await _context.Dishes
            .Include(d => d.Reviews)
            .FirstOrDefaultAsync(d => d.Id == model.DishId);

        if (dish == null)
        {
            return NotFound();
        }

        var review = new Review
        {
            UserId = userId,
            DishId = model.DishId,
            Text = model.Text,
            Rating = (byte)model.Rating, // Конвертація в byte
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction("DishDetails", new { id = model.DishId });
    }
    private IActionResult RedirectToCart() =>
        RedirectToAction("Index", "Cart");

    #endregion
}