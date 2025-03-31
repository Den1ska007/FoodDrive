using System;
using System.Collections.Generic;
using FoodDrive.Interfaces;
namespace FoodDrive.Models
{
    public class Order : BaseEntity
    {
        public Customer User { get; set; }
        public List<Dish> Products { get; set; }
        public decimal TotalPrice { get; set; }
        public Status Status { get; set; }
        public TimeSpan Time { get; set; }
        public Order() { }
        public Order(Customer user, List<Dish> products, Status status, TimeSpan time)
        {
            User = user;
            Products = products;
            TotalPrice = CalculateTotal();
            Status = status;
            Time = time;
        }

        private decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var product in Products)
            {
                total += product.Price;
            }
            return total;
        }
    }
    public class OrderRepository : Repository<Order> { }
}