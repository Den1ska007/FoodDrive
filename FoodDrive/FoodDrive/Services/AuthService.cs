// Services/AuthService.cs
using FoodDrive.Models;
using BCrypt.Net;
using FoodDrive.Interfaces;

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
        // Шукаємо адміністратора
        var admin = _adminRepository.GetAll()
            .FirstOrDefault(u => u.Name == username);

        if (admin != null)
            return admin;

        // Шукаємо користувача
        var customer = _customerRepository.GetAll()
            .FirstOrDefault(u => u.Name == username);

        if (customer != null)
            return customer;

        return null;
    }

    public void Register(User user)
    {
        _userRepository.Add(user);
    }
}