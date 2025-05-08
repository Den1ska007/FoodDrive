// Controllers/AccountController.cs
using FoodDrive.Models;
using FoodDrive.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodDrive.Models.ViewModels;

public class AccountController : Controller
{
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;
    private readonly IRepository<Admin> _adminRepository;
    private readonly IRepository<Customer> _customerRepository;
    public AccountController(IRepository<Admin> adminRepository,
        IRepository<Customer> customerRepository,
        UserRepository userRepository,
        AuthService authService)
    {
        _userRepository = userRepository;
        _adminRepository = adminRepository;
        _customerRepository = customerRepository;
        _authService = authService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        
        var user = _authService.Authenticate(model.Username, model.Password);
        if (user == null)
        {
            ViewData["ErrorMessage"] = "Невірний логін або пароль";
            return View(model);
        }

        // Створюємо Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        // Автентифікуємо користувача
        await HttpContext.SignInAsync("CookieAuth", principal);

        // Перенаправляємо за роллю
        return RedirectToAction("Index", user.Role);
    }

    [Authorize]
    public IActionResult Profile()
    {
        // Отримання поточного користувача
        var username = User.Identity.Name;
        var user = _userRepository.GetByUsername(username);

        // Перевірка наявності користувача
        if (user == null)
            return RedirectToAction("Login");

        // Створення ViewModel
        var model = new ProfileViewModel
        {
            Name = user.Name,
            Role = user.Role,
            Address = user.Address,
            RegistrationDate = user.CreatedAt
        };

        return View(model);
    }
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View();

    // Controllers/AccountController.cs
    [HttpPost]
    [AllowAnonymous]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        if (_userRepository.GetAll().Any(u => u.Name == model.Name))
        {
            ModelState.AddModelError("Name", "Це ім'я користувача вже використовується.");
            return View(model);
        }
        if (model.Role == "Admin")
        {
            var admin = new Admin(model.Name, model.Password, model.Address);
            _adminRepository.Add(admin);
            _userRepository.Add(admin);
        }
        else
        {
            var customer = new Customer(model.Name, model.Password, model.Address);
            _customerRepository.Add(customer);
            _userRepository.Add(customer);
        }

        return RedirectToAction("Login");
    }
    [HttpPost]
    [Authorize]
    public IActionResult EditProfile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var username = User.Identity.Name;
            var user = _userRepository.GetByUsername(username);
            if (user != null)
            {
                user.Name = model.Name;
                user.Address = model.Address;
                
                if (user is Customer customer)
                {
                    customer.Balance = model.Balance;
                }

                _userRepository.Update(user);
            }
            return RedirectToAction("Profile");
        }

        return View(model);
    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }
}