using System;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class Review : BaseEntity
    {
        public int Id { get; set; }
        public Customer User { get; set; }
        public Dish Dish { get; set; }
        public string Text { get; set; }
        public byte Rating { get; set; }
        public DateTime Date { get; set; }

        public Review(Customer user, Dish dish, string text, byte rating, DateTime date)
        {
            User = user;
            Dish = dish;
            Text = text;
            Rating = rating;
            Date = date;
        }
    }
    public class ReviewRepository : Repository<Review> { }
}