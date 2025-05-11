using FoodDrive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Customer")]
public class CartController : Controller
{
    private readonly CartRepository _cartRepository;
    private readonly DishRepository _dishRepository;
    private readonly UserRepository _userRepository;

    public CartController(CartRepository cartRepository, DishRepository dishRepository)
    {
        _cartRepository = cartRepository;
        _dishRepository = dishRepository;
    }

    [Authorize]
    public IActionResult Index()
    {
        // Отримуємо значення клейма
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Перевірка наявності клейма
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return RedirectToAction("Login", "Account");
        }

        // Перевірка коректності формату ID
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction("Error", "Home", new { message = "Невірний формат ID користувача" });
        }

        // Логіка отримання кошика
        var cart = _cartRepository.GetByUserId(userId);
        return View(cart ?? new Cart());
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddToCart(int dishId, int quantity = 1)
    {
        if (quantity < 1) quantity = 1;

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var dish = _dishRepository.GetById(dishId);

        if (dish == null || dish.Stock < quantity)
        {
            TempData["ErrorMessage"] = "Страва недоступна";
            return RedirectToAction("Index", "Customer");
        }

        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            cart.id = _cartRepository.GetAll().Any()
                ? _cartRepository.GetAll().Max(c => c.id) + 1
                : 1; // Фікс ID
            _cartRepository.Add(cart);
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
                id = cart.Items.Any()
                    ? cart.Items.Max(i => i.id) + 1
                    : 1,
                CartId = cart.id,
                DishId = dishId,
                Quantity = quantity,
                Dish = dish
            });
        }

        _cartRepository.Update(cart); // Виправлено оновлення кошика
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize]
    public IActionResult RemoveFromCart(int itemId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var cart = _cartRepository.GetByUserId(userId);

        if (cart != null)
        {
            var item = cart.Items.FirstOrDefault(i => i.id == itemId);
            if (item != null)
            {
                cart.Items.Remove(item);
                _cartRepository.Update(cart);
            }
        }

        return RedirectToAction("Index");
    }
}
