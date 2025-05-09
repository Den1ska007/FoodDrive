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
        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "Кількість має бути більше нуля";
            return RedirectToAction("Index", "Customer");
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var dish = _dishRepository.GetById(dishId);

        if (dish == null)
        {
            TempData["ErrorMessage"] = "Страва не знайдена";
            return RedirectToAction("Index", "Customer");
        }

        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _cartRepository.Add(cart);
            _cartRepository.Update(cart); // Збереження нової корзини
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.DishId == dishId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            _cartRepository.Update(cart);
        }
        else
        {
            var newCartItem = new CartItem
            {
                CartId = cart.id,
                DishId = dishId,
                Quantity = quantity,
                Dish = dish
            };
            cart.Items.Add(newCartItem);
            _cartRepository.Update(cart);
        }

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
