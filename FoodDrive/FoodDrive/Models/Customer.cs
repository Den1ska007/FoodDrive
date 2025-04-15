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
            // 3. Инициализируем коллекцию
            Orders = new List<Order>();

            // 4. Установим роль по умолчанию
            Role = "Customer";
        }
        public Customer(string name, string password, string address, List<Order> orders)
            : base(name, password, "Customer", address)
        {
            Orders = orders;
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