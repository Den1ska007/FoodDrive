using FoodDrive.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Customer")]
public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return View(await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Dish)
            .FirstOrDefaultAsync(c => c.UserId == userId) ?? new Cart { UserId = userId });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int dishId, int quantity = 1)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var cart = await GetOrCreateCart(userId);
        var dish = await _context.Dishes.FindAsync(dishId);

        if (dish?.Stock < quantity)
        {
            TempData["Error"] = "Недостатня кількість";
            return RedirectToAction("Index", "Menu");
        }

        var item = cart.Items.FirstOrDefault(i => i.DishId == dishId);
        if (item != null) item.Quantity += quantity;
        else cart.Items.Add(new CartItem { DishId = dishId, Quantity = quantity });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<Cart> GetOrCreateCart(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null) return cart;

        cart = new Cart { UserId = userId };
        await _context.Carts.AddAsync(cart);
        return cart;
    }
}