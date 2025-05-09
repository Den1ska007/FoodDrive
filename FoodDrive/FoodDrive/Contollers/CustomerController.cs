using FoodDrive.Interfaces;
using FoodDrive.Models;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly IRepository<Dish> _dishRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly CartRepository _cartRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly UserService _userService;

    public CustomerController(
        IRepository<Dish> dishRepository,
        IRepository<Order> orderRepository,
        CartRepository cartRepository,
        CustomerRepository customerRepository,
        UserService userService)
    {
        _dishRepository = dishRepository;
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _customerRepository = customerRepository;
        _userService = userService;
    }

    public IActionResult Index()
    {
        var menu = _dishRepository.GetAll();
        return View(menu);
    }

    public IActionResult Orders()
    {
        var userId = GetCurrentUserId();
        var orders = _orderRepository.GetAll()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        return View(orders);
    }

    public IActionResult OrderDetails(int id)
    {
        var order = _orderRepository.GetById(id);
        if (order == null || order.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
        {
            return NotFound();
        }
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddToCart(int dishId, int quantity = 1)
    {
        if (quantity < 1) quantity = 1;

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var dish = _dishRepository.GetById(dishId);

        if (dish == null)
        {
            TempData["ErrorMessage"] = "Страва не знайдена";
            return RedirectToAction("Index");
        }

        var cart = _cartRepository.GetByUserId(userId) ?? new Cart { UserId = userId };
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

        _cartRepository.Update(cart);
        TempData["SuccessMessage"] = "Страву додано до кошика";
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public IActionResult Checkout()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Кошик порожній";
            return RedirectToAction("Index", "Cart");
        }

        var customer = _customerRepository.GetById(userId);
        if (customer == null)
        {
            TempData["Error"] = "Користувача не знайдено";
            return RedirectToAction("Error", "Home");
        }

        var model = new CheckoutViewModel
        {
            Items = cart.Items,
            TotalPrice = cart.Total,
            CustomerName = customer.Name,
            CustomerAddress = customer.Address
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult ConfirmOrder()
    {
        var userId = GetCurrentUserId();
        var cart = _cartRepository.GetByUserId(userId);
        var customer = _customerRepository.GetById(userId);

        if (cart == null || !cart.Items.Any() || customer == null)
        {
            TempData["Error"] = "Помилка. Спробуйте ще раз.";
            return RedirectToAction("Index", "Cart");
        }

        using (var transaction = new TransactionScope()) // Додано транзакцію
        {
            try
            {
                // Перевірка балансу
                if (customer.Balance < cart.Total)
                {
                    TempData["Error"] = "Недостатньо коштів на рахунку.";
                    return RedirectToAction("Checkout");
                }

                // Оновлення запасів страв
                foreach (var item in cart.Items)
                {
                    var dish = _dishRepository.GetById(item.DishId);
                    if (dish == null || dish.Stock < item.Quantity)
                    {
                        TempData["Error"] = $"Страва '{dish?.Name}' недоступна в потрібній кількості";
                        return RedirectToAction("Checkout");
                    }
                    dish.Stock -= item.Quantity;
                    _dishRepository.Update(dish);
                }

                // Створення замовлення
                var order = new Order
                {
                    UserId = customer.id,
                    Products = cart.Items.Select(i => i.Dish).ToList(),
                    TotalPrice = cart.Total,
                    Status = Status.Pending,
                    OrderDate = DateTime.Now
                };
                order.id = _orderRepository.GetAll().Any()
                    ? _orderRepository.GetAll().Max(o => o.id) + 1
                    : 1; // Фікс генерації ID
                _orderRepository.Add(order);

                // Оновлення балансу
                customer.Balance -= cart.Total;
                _customerRepository.Update(customer);

                // Очищення кошика
                _cartRepository.Remove(cart);

                transaction.Complete(); // Підтвердження транзакції

                TempData["Success"] = "Замовлення успішно оформлено!";
                return RedirectToAction("Orders");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Сталася помилка: " + ex.Message;
                return RedirectToAction("Checkout");
            }
        }
    }
    [HttpGet]
    public IActionResult EditProfile()
    {
        var user = _userService.GetUserProfile(GetCurrentUserId());
        var model = new EditProfileViewModel
        {
            Name = user.Name,
            Address = user.Address
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _userService.GetUserProfile(GetCurrentUserId());

        // Перевірка поточного пароля
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
            {
                ModelState.AddModelError("CurrentPassword", "Невірний поточний пароль");
                return View(model);
            }
        }

        // Оновлення даних
        user.Name = model.Name;
        user.Address = model.Address;

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            user.Password = model.NewPassword; // Автоматичне хешування через сеттер
        }

        if (_userService.UpdateUserProfile(user))
        {
            TempData["SuccessMessage"] = "Профіль успішно оновлено";
            return RedirectToAction("Profile");
        }

        ModelState.AddModelError("", "Помилка при оновленні профілю");
        return View(model);
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}