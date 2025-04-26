using System;
using System.Collections.Generic;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class Customer : User
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public Customer() : base()
        {
            
            Orders = new List<Order>();

            
            Role = "Customer";
        }
        public Customer(string name, string password, string address)
            : base(name, password, "Customer", address)
        {
            
        }
        
        public override string GetInfo()
        {
            return $"{Name}_{Password}_{Role}_{Orders.Count}";
        }
    }
    public class CustomerRepository : Repository<Customer>
    {
        public CustomerRepository(IDataStorage<Customer> storage) : base(storage)
        {
        }
    }

}