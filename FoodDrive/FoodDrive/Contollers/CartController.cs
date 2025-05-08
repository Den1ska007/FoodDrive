using FoodDrive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class CartController : Controller
{
    private readonly CartRepository _cartRepository;
    private readonly DishRepository _dishRepository;

    public CartController(CartRepository cartRepository, DishRepository dishRepository)
    {
        _cartRepository = cartRepository;
        _dishRepository = dishRepository;
    }

    [Authorize]
    public IActionResult Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var cart = _cartRepository.GetByUserId(userId);
        return View(cart ?? new Cart());
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddToCart(int dishId, int quantity = 1)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var dish = _dishRepository.GetById(dishId);

        if (dish == null) return NotFound();

        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _cartRepository.Add(cart);
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
