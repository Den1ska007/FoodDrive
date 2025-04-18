using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDrive.Models;
using FoodDrive.Services;
using System.Security.Claims;
using System.Linq;
using FoodDrive.Interfaces;
using System.Collections.Generic;
[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly IRepository<Dish> _dishRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly UserService _userService;

    public CustomerController(
        IRepository<Dish> dishRepository,
        IRepository<Order> orderRepository,
        UserService userService)
    {
        _dishRepository = dishRepository;
        _orderRepository = orderRepository;
        _userService = userService;
    }

    public IActionResult Index()
    {
        var dishes = _dishRepository.GetAll();
        return View(dishes);
    }

    public IActionResult Cart()
    {
        return View();
    }

    public IActionResult Orders()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var userOrders = _orderRepository.GetAll()
            .Where(o => o.User.id == userId)
            .OrderByDescending(o => o.Time);

        return View(userOrders);
    }

    [HttpPost]
    public IActionResult PlaceOrder(List<CartItem> items)
    {
        if (!items.Any())
        {
            return RedirectToAction("Cart");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _userService.GetUserProfile(userId) as Customer;

        var order = new Order
        {
            User = user,
            Products = items.Select(i =>
            {
                var dish = _dishRepository.GetById(i.DishId);
                return dish;
            }).ToList(),
            Status = Status.Pending,
            Time = DateTime.Now.TimeOfDay
        };

        // Розраховуємо загальну суму
        order.TotalPrice = order.Products.Sum(p => p.Price);

        _orderRepository.Add(order);
        return RedirectToAction("OrderConfirmation", new { id = order.id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        var order = _orderRepository.GetById(id);
        return View(order);
    }
}