using System;
using System.Collections.Generic;

namespace FoodDrive.Models
{
    public class Customer : User
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public Customer(int id, string name, string password, int mobilenum, string adres, List<Order> orders)
            : base(id, name, password, "Customer", mobilenum, adres)
        {
            Orders = orders;
        }
        
        public override string GetInfo()
        {
            return $"{Id}_{Name}_{Password}_{Role}_{Orders.Count}";
        }
    }
    
    public class CustomerRepository : Repository<Customer> { }

}