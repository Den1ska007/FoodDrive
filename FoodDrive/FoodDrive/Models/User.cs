using System;
using BCrypt.Net;
using FoodDrive.Interfaces;


namespace FoodDrive.Models
{
    // Абстрактний клас User
    public abstract class User : BaseEntity
    {
        public string Name { get; set; }
        
        private string _password;
        public string Password
        {
            get => _password;
            set => _password = BCrypt.Net.BCrypt.HashPassword(value); // Хешуємо пароль перед збереженням
        }

        public string Role { get; protected set; }
        public string Address { get; set; }
        protected User()
        {
            Name = string.Empty;
            Password = string.Empty;
            Role = string.Empty;
            Address = string.Empty;
        }
        public User(string name, string password, string role, string address)
        {
            Name = name;
            Password = password;
            Role = role;
            Address = address;
        }
        public abstract string GetInfo(); // Абстрактний метод

        public bool VerifyPassword(string enteredPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, Password);
        }
    }

    public class UserRepository : Repository<User>
    {
        public UserRepository(IDataStorage<User> storage) : base(storage)
        {
        }

        // Додаткові методи для роботи з користувачами, якщо потрібно
        public User GetByUsername(string username)
        {
            return GetAll().FirstOrDefault(u => u.Name == username);
        }
    }
}