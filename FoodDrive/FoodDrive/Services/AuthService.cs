// Services/AuthService.cs
using FoodDrive.Models;
using BCrypt.Net;
using FoodDrive.Interfaces;
public interface IAuthService
{
    User? Authenticate(string username, string password);
    void Register(User user);
}
public class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly IRepository<Admin> _adminRepository;
    private readonly IRepository<Customer> _customerRepository;
    public AuthService(UserRepository userRepository,
        IRepository<Admin> adminRepository,
        IRepository<Customer> customerRepository)
    {
        _userRepository = userRepository;
        _adminRepository = adminRepository;
        _customerRepository = customerRepository;
    }

    public User? Authenticate(string username, string password)
    {

        var user = _userRepository.GetByUsername(username)
        ?? _adminRepository.GetAll().FirstOrDefault(u => u.Name == username);
        if (user == null) return null;
        return BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password)
                ? user
                : null;
    }

    public void Register(User user)
    {
        _userRepository.Add(user);
    }
}