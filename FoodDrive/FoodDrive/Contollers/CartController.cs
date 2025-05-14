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

        var cartItem = new CartItem
        {
            DishId = dishId,
            Quantity = quantity,
            CartId = cart.Id
        };

        await _context.CartItems.AddAsync(cartItem);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(int itemId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                TempData["Error"] = "Кошик не знайдено";
                return RedirectToAction("Index");
            }

            var itemToRemove = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (itemToRemove == null)
            {
                TempData["Error"] = "Товар не знайдено в кошику";
                return RedirectToAction("Index");
            }
            _context.CartItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар успішно видалено";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Сталася помилка. Спробуйте пізніше";
            return RedirectToAction("Index");
        }
    }
    private async Task<Cart> GetOrCreateCart(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null) return cart;
        var newCart = new Cart { UserId = userId };
        await _context.Carts.AddAsync(newCart);
        await _context.SaveChangesAsync();

        return newCart;
    }
}