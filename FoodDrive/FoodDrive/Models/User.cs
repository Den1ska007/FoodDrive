using System;
using System.ComponentModel.DataAnnotations;
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
            set => _password = BCrypt.Net.BCrypt.EnhancedHashPassword(value, 13);
        }


        public string Role { get; set; }
        [Required(ErrorMessage = "Адреса обов'язкова")]
        [StringLength(200, ErrorMessage = "Адреса має бути до 200 символів")]
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
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
        public abstract string GetInfo();

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

        public User GetById(int id)
        {
            return _storage.Load()
                .FirstOrDefault(u => u.id == id);
        }
        public User GetByUsername(string username)
        {
            return GetAll().FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}