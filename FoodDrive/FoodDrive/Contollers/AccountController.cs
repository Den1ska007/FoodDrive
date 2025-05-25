using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodDrive.Entities;
using FoodDrive.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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

        if (user == null || _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Невірний логін або пароль");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role)
        };

        await HttpContext.SignInAsync(
            "CookieAuth",
            new ClaimsPrincipal(new ClaimsIdentity(claims, "CookieAuth")),
            new AuthenticationProperties { IsPersistent = true });

        return RedirectToAction("Index", user.Role);
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Users
            .Include(u => (u as Customer)!.Orders)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return RedirectToAction("Login");

        return View(new ProfileViewModel
        {
            Name = user.Name,
            Role = user.Role,
            Address = user.Address,
            Balance = user is Customer customer ? customer.Balance : 0,
            RegistrationDate = user.CreatedAt
        });
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _context.Users.AnyAsync(u => u.Name == model.Name))
        {
            ModelState.AddModelError("Name", "Це ім'я вже використовується");
            return View(model);
        }

        User newUser;

        if (model.Role == "Admin")
        {
            newUser = new Admin
            {
                Name = model.Name,
                Address = model.Address,
                PasswordHash = _passwordHasher.HashPassword(null, model.Password),
                Role = "Admin"
            };
        }
        else
        {
            newUser = new Customer
            {
                Name = model.Name,
                Address = model.Address,
                PasswordHash = _passwordHasher.HashPassword(null, model.Password),
                Role = "Customer",
                Balance = model.Balance
            };
        }

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult EditProfile() => View(new EditProfileViewModel());
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Customers.FindAsync(userId);

        if (user == null)
            return RedirectToAction("Login");

        // Оновлення імені та адреси
        user.Name = model.Name;
        user.Address = model.Address;

        // Зміна пароля (якщо введено новий)
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            // Перевірка поточного пароля
            if (string.IsNullOrEmpty(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Введіть поточний пароль");
                return View(model);
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword) != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("CurrentPassword", "Поточний пароль невірний");
                return View(model);
            }

            // Оновлення пароля
            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Profile");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }
}