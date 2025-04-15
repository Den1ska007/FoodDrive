// Controllers/AccountController.cs
using System.Security.Claims;
using FoodDrive.Interfaces;
using FoodDrive.Models;
using FoodDrive.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly AuthService _authService;
    private readonly UserService _userService;
    private readonly IRepository<User> _userRepository;

    public AccountController(AuthService authService, IRepository<User> userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _authService.Authenticate(username, password);
        if (user == null)
        {
            ModelState.AddModelError("", "Невірний логін або пароль");
            return View();
        }

        var principal = _authService.CreateClaimsPrincipal(user);
        await HttpContext.SignInAsync(principal);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password, string address)
    {
        var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Name == username);
        if (existingUser != null)
        {
            ModelState.AddModelError("", "Користувач з таким іменем вже існує");
            return View();
        }

        var newCustomer = new Customer
        {
            Name = username,
            Password = password,
            Address = address
        };

        _userRepository.Add(newCustomer);
        return RedirectToAction("Login");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Admin");
    }
    [Authorize]
    [Authorize]
    public IActionResult Profile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _userService.GetUserProfile(userId);
        return View(user);
    }
    [Authorize]
    public IActionResult EditProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _userService.GetUserProfile(userId);
        return View(user);
    }

    [Authorize]
    [HttpPost]
    public IActionResult EditProfile(User model)
    {
        if (ModelState.IsValid)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var success = _userService.UpdateUserProfile(userId, model.Name, model.Address);

            if (success)
                return RedirectToAction("Profile");

            ModelState.AddModelError("", "Не вдалося оновити профіль");
        }
        return View(model);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogoutConfirmed()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
    [Authorize(Roles = "Admin")]
    public IActionResult AdminPanel()
    {
        return View();
    }
}