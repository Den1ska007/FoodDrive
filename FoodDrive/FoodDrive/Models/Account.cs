namespace FoodDrive.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private static Dictionary<string, string> users = new(); // Фейкова БД

    [HttpPost("register")]
    public IActionResult Register(string username, string password)
    {
        if (users.ContainsKey(username))
            return BadRequest("Користувач вже існує");

        string hashedPassword = BCrypt.HashPassword(password);
        users[username] = hashedPassword;

        return Ok("Користувач зареєстрований!");
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        if (!users.TryGetValue(username, out string? storedHash))
            return Unauthorized("Користувач не знайдений");

        bool isValid = BCrypt.Verify(password, storedHash);
        return isValid ? Ok("Успішний вхід!") : Unauthorized("Невірний пароль");
    }
}

