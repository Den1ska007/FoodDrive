using System;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class Review : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Dish Dish { get; set; }
        public int DishId { get; set; }
        public string Text { get; set; }
        public byte Rating { get; set; }
        public DateTime Date { get; set; }

        public Review()
        {
            UserId = 0;
            User = null;
            Dish = null;
            Text = string.Empty;
            Rating = 0;
            Date = DateTime.MinValue;
        }

        public Review(int UserId, User user, Dish dish, string text, byte rating, DateTime date)
        {
            UserId = UserId;
            User = user;
            Dish = dish;
            Text = text;
            Rating = rating;
            Date = date;
        }
    }

    public class ReviewRepository : Repository<Review>
    {
        public ReviewRepository(IDataStorage<Review> storage) : base(storage)
        {
        }

    }
}