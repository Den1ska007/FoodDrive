// Controllers/CustomerController.cs
using FoodDrive.Interfaces;
using FoodDrive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly IRepository<Dish> _dishRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly OrderService _orderService;
    private readonly UserRepository _userRepository;
    public CustomerController(
        IRepository<Dish> dishRepository,
        IRepository<Order> orderRepository,
        OrderService orderService, 
        UserRepository userRepository)
    {
        _dishRepository = dishRepository;
        _orderRepository = orderRepository;
        _orderService = orderService;
        _userRepository = userRepository;
    }

    public IActionResult Index()
    {
        var menu = _dishRepository.GetAll();
        return View(menu);
    }
    public IActionResult Orders()
    {
        var username = User.Identity.Name;
        var user = _userRepository.GetByUsername(username);

        if (user == null) return RedirectToAction("Login", "Account");

        var orders = _orderRepository.GetAll().Where(o => o.User.id == user.id).ToList();

        return View(orders);
    }
    [HttpPost]
    public IActionResult PlaceOrder(Cart cart)
    {
        var result = _orderService.PlaceOrder(cart);
        if (result.Success)
            return RedirectToAction("OrderDetails", new { id = result.Order.id });

        TempData["Error"] = result.Message;
        return RedirectToAction("Cart");
    }
}