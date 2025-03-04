using System;

namespace FoodDrive.Models
{
    public class Review
    {
        public int Id { get; set; }
        public Customer User { get; set; }
        public Dish Dish { get; set; }
        public string Text { get; set; }
        public byte Rating { get; set; }
        public DateTime Date { get; set; }

        public Review(int id, Customer user, Dish dish, string text, byte rating, DateTime date)
        {
            Id = id;
            User = user;
            Dish = dish;
            Text = text;
            Rating = rating;
            Date = date;
        }
    }
}