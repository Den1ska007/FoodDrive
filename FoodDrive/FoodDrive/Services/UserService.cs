using FoodDrive.Entities;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> UpdateUserProfile(int userId, EditProfileViewModel model)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        try
        {
            user.Name = model.Name;
            user.Address = model.Address;

            if (!string.IsNullOrEmpty(model.NewPassword))
                user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetUserProfile(int userId)
    {
        return await _context.Users
            .Include(u => (u as Customer).Orders)
            .ThenInclude(o => o.Items)
            .ThenInclude(i => i.Dish)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}