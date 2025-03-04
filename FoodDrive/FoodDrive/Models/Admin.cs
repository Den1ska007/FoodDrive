using System;

namespace FoodDrive.Models
{
    public class Admin : User
    {
        public Admin(int id, string name, string password, int mobilenum, string adres)

            : base(id, name, password,"Admin", mobilenum, adres)
        {
           
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Admin: {Name}");
        }

        public void ManageProducts()
        {
            Console.WriteLine("Managing products...");
        }
    }
}