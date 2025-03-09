using System;
using System.Collections.Generic;
using FoodDrive.Interfaces;



namespace FoodDrive.Models
{
    public class Dish : IReviewable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
        public float Rating { get; set; }
         
        public Dish(int id, string name, string description, decimal price, Category category, int stock, List<Review> reviews, float rating )
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            Reviews = reviews;
            Rating = rating;
        }

        public void AddReview(Review review)
        {
            Reviews.Add(review);
            Console.WriteLine($"Review added for {Name}: {review.Text}");
        }
    }
}