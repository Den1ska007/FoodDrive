// Controllers/CustomerController.cs
using FoodDrive.Interfaces;
using FoodDrive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly IRepository<Dish> _dishRepository;
    private readonly IRepository<Order> _orderRepository;

    public CustomerController(
        IRepository<Dish> dishRepository,
        IRepository<Order> orderRepository)
    {
        _dishRepository = dishRepository;
        _orderRepository = orderRepository;
    }

    public IActionResult Index()
    {
        var menu = _dishRepository.GetAll();
        return View(menu);
    }

    public IActionResult Cart()
    {
        return View();
    }

    [HttpPost]
    public IActionResult PlaceOrder(List<CartItem> items)
    {
        var order = new Order
        {
            Products = items.Select(i => i.Dish).ToList(),
            Status = Status.Pending
        };
        _orderRepository.Add(order);
        return RedirectToAction("OrderStatus");
    }
}