// Services/AuthService.cs
using FoodDrive.Interfaces;
using FoodDrive.Models;
using System.Security.Claims;

public class AuthService
{
    private readonly IRepository<User> _userRepository;

    public AuthService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public User Authenticate(string username, string password)
    {
        var user = _userRepository.GetAll().FirstOrDefault(u => u.Name == username);
        if (user == null || !user.VerifyPassword(password))
            return null;

        return user;
    }

    public ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        return new ClaimsPrincipal(identity);
    }
}