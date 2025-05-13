using BCrypt.Net;
using FoodDrive.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    Task<User?> Authenticate(string username, string password);
    Task<bool> Register<TUser>(TUser user) where TUser : User;
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> Authenticate(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == username);

        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
            return null;

        return user;
    }

    public async Task<bool> Register<TUser>(TUser user) where TUser : User
    {
        try
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            if (user is Customer customer)
                await _context.Customers.AddAsync(customer);
            else if (user is Admin admin)
                await _context.Admins.AddAsync(admin);
            else
                return false;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}