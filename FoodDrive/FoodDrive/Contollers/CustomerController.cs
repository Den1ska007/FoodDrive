using FoodDrive.Entities;
using FoodDrive.Interfaces;
using FoodDrive.Models;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Transactions;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public CustomerController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var menu = await _context.Dishes.ToListAsync();
        return View(menu);
    }

    public async Task<IActionResult> Orders()
    {
        var userId = GetCurrentUserId();
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Dish)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        var userId = GetCurrentUserId();
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Dish)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }


    [HttpPost]
    public async Task<IActionResult> AddToCart(int dishId, int quantity = 1)
    {
        var userId = GetCurrentUserId();
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        var dish = await _context.Dishes.FindAsync(dishId);
        if (dish == null || dish.Stock < quantity)
        {
            TempData["Error"] = "Страва недоступна";
            return RedirectToAction("Index");
        }

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await _context.Carts.AddAsync(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.DishId == dishId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                DishId = dishId,
                Quantity = quantity,
                Dish = dish
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("ViewCart");
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Dish)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Кошик порожній";
            return RedirectToAction("Index", "Cart");
        }

        var customer = await _userManager.FindByIdAsync(userId.ToString()) as Customer;
        if (customer == null)
        {
            TempData["Error"] = "Користувача не знайдено";
            return RedirectToAction("Error", "Home");
        }

        var model = new CheckoutViewModel
        {
            Items = cart.Items,
            TotalPrice = cart.Items.Sum(i => i.Dish.Price * i.Quantity),
            CustomerName = customer.Name,
            CustomerAddress = customer.Address
        };

        return View(model);
    }

    public async Task<IActionResult> DishDetails(int id)
    {
        var dish = await _context.Dishes
            .Include(d => d.Reviews)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dish == null)
        {
            TempData["ErrorMessage"] = "Страва не знайдена";
            return RedirectToAction("Index");
        }
        return View(dish);
    }
    [HttpPost]
    public async Task<IActionResult> ConfirmOrder()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Кошик порожній";
                return RedirectToAction("Index");
            }

            var customer = await _userManager.FindByIdAsync(userId.ToString()) as Customer;
            if (customer == null)
            {
                TempData["Error"] = "Користувача не знайдено";
                return RedirectToAction("Error", "Home");
            }

            var totalPrice = cart.Items.Sum(i => i.Dish.Price * i.Quantity);
            if (customer.Balance < totalPrice)
            {
                TempData["Error"] = "Недостатньо коштів на рахунку";
                return RedirectToAction("Checkout");
            }

            // Оновлення запасів страв
            foreach (var item in cart.Items)
            {
                var dish = await _context.Dishes.FindAsync(item.DishId);
                dish.Stock -= item.Quantity;
                if (dish.Stock < 0)
                {
                    throw new InvalidOperationException("Недостатній запас страв");
                }
            }

            // Створення замовлення
            var order = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                Status = Status.Pending,
                OrderDate = DateTime.UtcNow,
                Items = cart.Items.Select(i => new OrderItem
                {
                    DishId = i.DishId,
                    Quantity = i.Quantity
                }).ToList()
            };

            customer.Balance -= totalPrice;

            await _context.Orders.AddAsync(order);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Замовлення успішно оформлено!";
            return RedirectToAction("Orders");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Помилка: {ex.Message}";
            return RedirectToAction("Checkout");
        }
    }
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User) as Customer;
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new EditProfileViewModel
        {
            Name = user.Name,
            Address = user.Address
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User) as Customer;
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Перевірка пароля
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError("CurrentPassword", "Невірний поточний пароль");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        // Оновлення даних
        user.Name = model.Name;
        user.Address = model.Address;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        TempData["SuccessMessage"] = "Профіль успішно оновлено";
        return RedirectToAction("Profile", "Account");
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}