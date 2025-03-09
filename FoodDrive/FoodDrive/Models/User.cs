using System;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    // Абстрактний клас User
    public abstract class User : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        protected string Password { get; set; }
        public string Role { get; protected set; }
        public int MobileNum { get; set; }
        public string Adres { get; set; }
        public User(int id, string name, string password, string role, int mobilenum, string adres)
        {
            Id = id;
            Name = name;
            Password = password;
            Role = role;
            MobileNum = mobilenum;
            Adres = adres;
        }

        public abstract void DisplayInfo(); // Абстрактний метод
    }
}