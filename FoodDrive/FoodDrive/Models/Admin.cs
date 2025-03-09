using System;
using FoodDrive.Models;

namespace FoodDrive.Models
{
    public class Admin : User
    {
        public Admin(int id, string name, string password, int mobilenum, string adres)

            : base(id, name, password,"Admin", mobilenum, adres)
        {
           
        }

        public override string GetInfo()
        {
            return $"{Id}_{Name}_{Password}_{Role}";
        }

        public void ManageProducts()
        {
            Console.WriteLine("Managing products...");
        }
    }
    public class AdminRepository : Repository<Admin> { }
}
