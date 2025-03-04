using System;
using System.Collections.Generic;

namespace FoodDrive.Models
{
    public class Customer : User
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public Customer(int id, string name, string password, int mobilenum, string adres)
            : base(id, name, password, "Customer", mobilenum, adres)
        {
           
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Customer: {Name}");
        }
    }
}