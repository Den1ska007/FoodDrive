using FoodDrive.Models.ViewModels;
using FoodDrive.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodDrive.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _hasher;
    private readonly AuthService _authService;
    public AccountController(AppDbContext context, IPasswordHasher<User> hasher, AuthService authService)
    {
        _context = context;
        _hasher = hasher;
        _authService = authService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == model.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true }
            );

        return RedirectToAction("Index", user.Role);
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        // Отримуємо ID користувача з клеймів
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction("Login");
        }

        // Шукаємо користувача з пов'язаними даними
        var user = await _context.Users
            .Include(u => (u as Customer).Orders)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ProfileViewModel
        {
            Name = user.Name,
            Role = user.Role,
            Address = user.Address,
            Balance = user is Customer customer ? customer.Balance : 0,
            RegistrationDate = user.CreatedAt
        };

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new Customer
        {
            Name = model.Name,
            Address = model.Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = "Customer"
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Отримуємо повну інформацію про користувача
        var user = await _context.Users
            .OfType<Customer>()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        try
        {
            // Оновлюємо основні поля
            user.Name = model.Name;
            user.Address = model.Address;

            // Оновлюємо баланс тільки для Customer
            if (user is Customer customer)
            {
                customer.Balance = model.Balance;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Профіль успішно оновлено";
            return RedirectToAction("Profile");
        }
        catch (DbUpdateException ex)
        {
            ModelState.AddModelError("", "Помилка при збереженні змін: " + ex.Message);
            return View(model);
        }
    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }
}