using FoodDrive.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AdminController(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<IActionResult> ListAdmin() => View(await _context.Admins.ToListAsync());

    public IActionResult CreateAdmin() => View();

    [HttpPost]
    public async Task<IActionResult> CreateAdmin(Admin admin)
    {
        if (!ModelState.IsValid) return View(admin);

        admin.PasswordHash = _passwordHasher.HashPassword(admin, admin.PasswordHash);
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ListAdmin));
    }

    public async Task<IActionResult> EditAdmin(int id) =>
        View(await _context.Admins.FindAsync(id) ?? throw new InvalidOperationException());

    [HttpPost]
    public async Task<IActionResult> EditAdmin(Admin admin)
    {
        if (!ModelState.IsValid) return View(admin);

        var existing = await _context.Admins.FindAsync(admin.Id);
        if (existing == null) return NotFound();

        existing.Name = admin.Name;
        if (!string.IsNullOrEmpty(admin.PasswordHash))
            existing.PasswordHash = _passwordHasher.HashPassword(existing, admin.PasswordHash);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ListAdmin));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAdmin(int id)
    {
        var admin = await _context.Admins.FindAsync(id);
        if (admin == null) return NotFound();

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ListAdmin));
    }
}