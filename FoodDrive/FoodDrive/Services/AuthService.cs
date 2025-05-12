using BCrypt.Net;
using FoodDrive.Entities;
using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    Task<User?> Authenticate(string username, string password);
    Task<bool> Register(User user);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Authenticate(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // Replace EnhancedVerify with Verify
            return null;

        return user;
    }

    public async Task<bool> Register(User user)
    {
        try
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash); // Replace EnhancedHashPassword with HashPassword
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
