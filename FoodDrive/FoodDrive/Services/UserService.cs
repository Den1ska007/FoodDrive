using FoodDrive.Entities;
using FoodDrive.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
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
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            }

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
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}