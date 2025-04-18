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
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserService userService,
        AuthService authService,
        IRepository<User> userRepository,
        ILogger<AccountController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Логін та пароль обов'язкові");
                return View();
            }

            var user = _authService.Authenticate(username, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Невірний логін або пароль");
                return View();
            }

            var principal = _authService.CreateClaimsPrincipal(user);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToLocal(returnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при вході для користувача {Username}", username);
            ModelState.AddModelError("", "Сталася помилка при спробі входу");
            return View();
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new User());  // Повертаємо модель User замість RegisterViewModel
    }

    [HttpPost]
    public IActionResult Register(User model, string role)
    {
        if (ModelState.IsValid)
        {
            User newUser = role == "Admin"
                ? new Admin(model.Name, model.Password, model.Address)
                : new Customer(model.Name, model.Password, model.Address, new List<Order>());

            _userRepository.Add(newUser);
            return RedirectToAction("Login");
        }
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Profile()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var user = _userService.GetUserProfile(userId);
            if (user == null) return NotFound();

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні профілю");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpGet]
    public IActionResult EditProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var user = _userService.GetUserProfile(userId);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                Name = user.Name,
                Address = user.Address
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні профілю для редагування");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditProfile(EditProfileViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var success = _userService.UpdateUserProfile(userId, model.Name, model.Address);
            if (!success)
            {
                ModelState.AddModelError("", "Не вдалося оновити профіль");
                return View(model);
            }

            return RedirectToAction("Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при оновленні профілю");
            ModelState.AddModelError("", "Сталася помилка при оновленні профілю");
            return View(model);
        }
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

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) // Fixed missing closing parenthesis  
            return 0;

        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Index");
    }

    
}

// Додаткові моделі для View
public class RegisterViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Address { get; set; }
}

public class EditProfileViewModel
{
    public string Name { get; set; }
    public string Address { get; set; }
}