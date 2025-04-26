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
        public Order()
        {
            Products = new List<Dish>();
        }
        public Order(Customer user, List<Dish> products, Status status, TimeSpan time)
        {
            User = user;
            Products = products;
            TotalPrice = CalculateTotal();
            Status = status;
            Time = time;
        }

        public decimal CalculateTotal()
        {
            if (Products == null || !Products.Any())
                return 0;

            return Products.Sum(p => p.Price);
        }
    }
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(IDataStorage<Order> storage) : base(storage)
        {
        }
    }
}