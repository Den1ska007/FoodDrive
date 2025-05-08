using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class Customer : User
    {
        [Range(0, double.MaxValue, ErrorMessage = "Баланс не може бути від'ємним")]
        public decimal Balance { get; set; }
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
        public void UpdateBalance(int customerId, decimal amount)
        {
            var customer = GetById(customerId);
            if (customer != null)
            {
                customer.Balance += amount;
                Update(customer);
            }
        }
    }

}