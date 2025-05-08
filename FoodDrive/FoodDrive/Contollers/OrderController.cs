using FoodDrive.Interfaces;
using FoodDrive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodDrive.Models.ViewModels;
public class OrderController : Controller
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Dish> _dishRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly CartRepository _cartRepository;

    public OrderController(IRepository<Order> orderRepository,
                           IRepository<Dish> dishRepository,
                           IRepository<Customer> customerRepository,
                           CartRepository cartRepository)
    {
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _customerRepository = customerRepository;
        _cartRepository = cartRepository;
    }

    [Authorize]
    public IActionResult Checkout()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var cart = _cartRepository.GetByUserId(userId);

        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var customer = _customerRepository.GetById(userId);
        if (customer == null)
        {
            return RedirectToAction("Login", "Account");
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var cart = _cartRepository.GetByUserId(userId);

        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Ваш кошик порожній!";
            return RedirectToAction("Index", "Cart");
        }

        // Обробка замовлення
        var order = new Order
        {
            User = _customerRepository.GetById(userId),
            Products = cart.Items.Select(i => i.Dish).ToList(),
            TotalPrice = cart.Total,
            Status = Status.Pending
        };

        // Додавання замовлення в базу даних
        _orderRepository.Add(order);

        // Очищення кошика
        _cartRepository.Remove(cart);

        TempData["Success"] = "Ваше замовлення підтверджено!";
        return RedirectToAction("Index", "Customer");  // Переходимо на сторінку клієнта після підтвердження
    }
    public IActionResult OrderDetails(int id)
    {
        var order = _orderRepository.GetById(id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
