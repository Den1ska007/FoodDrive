using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public List<int> ProductIds { get; set; } = new List<int>();

        [JsonIgnore]
        public List<Dish> Products { get; set; } = new List<Dish>();

        public decimal TotalPrice { get; set; }
        public Status Status { get; set; }
        public DateTime OrderDate { get; set; }

        // Конструктор за замовчуванням
        public Order()
        {
            Products = new List<Dish>();
            OrderDate = DateTime.Now;
        }

        // Конструктор з параметрами
        public Order(int userId, Customer user, List<Dish> products, Status status)
        {
            UserId = userId;
            User = user;
            Products = products ?? new List<Dish>();
            TotalPrice = CalculateTotal();
            Status = status;
            OrderDate = DateTime.Now;
        }

        // Розрахунок загальної суми замовлення
        public decimal CalculateTotal()
        {
            return Products.Sum(p => p.Price);
        }
    }

    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(IDataStorage<Order> storage) : base(storage)
        {
        }

        public override void Add(Order entity)
        {
            // Генеруємо унікальний ID
            entity.id = _entities.Any() ? _entities.Max(o => o.id) + 1 : 1;
            _entities.Add(entity);
            _storage.Save(_entities); // Зберігаємо зміни
        }

    }
}