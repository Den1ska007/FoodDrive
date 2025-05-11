using System;
using FoodDrive.Interfaces;
using FoodDrive.Models;

namespace FoodDrive.Models
{
    public class Admin : User
    {
        public Admin() : base("default", "default", "Admin", "default")
        {
        }
        public Admin(string name, string password, string address)
            : base(name, password, "Admin", address)
        {
           
        }

        public override string GetInfo()
        {
            return $"{Name}_{Password}_{Role}_{Address}";
        }

        public void ManageProducts()
        {
            Console.WriteLine("Managing products...");
        }
    }
    public class AdminRepository : Repository<Admin>
    {
        public AdminRepository(IDataStorage<Admin> storage) : base(storage)
        {
        }
    }
}
