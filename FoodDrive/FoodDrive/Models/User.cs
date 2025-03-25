using System;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    // Абстрактний клас User
    public abstract class User : BaseEntity
    {
        public string Name { get; set; }
        protected string Password { get; set; }
        public string Role { get; protected set; }
        public int MobileNum { get; set; }
        public string Adres { get; set; }
        public User() :base() { }
        public User(string name, string password, string role, int mobilenum, string adres)
        {
            Name = name;
            Password = password;
            Role = role;
            MobileNum = mobilenum;
            Adres = adres;
        }
        private static int _latestId = 0;
        public abstract string GetInfo(); // Абстрактний метод
    }
    public class UserRepository : Repository<User> { }
}