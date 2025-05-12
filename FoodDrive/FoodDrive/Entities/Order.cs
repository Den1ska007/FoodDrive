using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using FoodDrive.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDrive.Entities
{
    [Table("orders")]
    public class Order
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        [Column("total_price")]
        [Precision(10, 2)]
        public decimal TotalPrice { get; set; }

        [Column("status")]
        [StringLength(20)]
        public Status Status { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
    public class OrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public Order GetById(int id)
        {
            return _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Dish)
                .FirstOrDefault(o => o.Id == id);
        }
    }
}