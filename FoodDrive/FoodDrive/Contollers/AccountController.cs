using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodDrive.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(AppDbContext context, IPasswordHasher<User> passwordHasher)
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

        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError(string.Empty, "Невірні облікові дані");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties { IsPersistent = true });

        return RedirectToAction("Index", user.Role);
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users
            .Include(u => (u as Customer)!.Orders)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user == null ? RedirectToAction("Login") : View(new ProfileViewModel
        {
            Name = user.Name,
            Role = user.Role,
            Address = user.Address,
            Balance = user is Customer customer ? customer.Balance : 0,
            RegistrationDate = user.CreatedAt
        });
    }

    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var customer = new Customer
        {
            Name = model.Name,
            Address = model.Address,
            PasswordHash = _passwordHasher.HashPassword(null!, model.Password),
            Role = "Customer"
        };

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}