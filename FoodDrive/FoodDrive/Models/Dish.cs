using System;
using System.Collections.Generic;
using FoodDrive.Interfaces;



namespace FoodDrive.Models
{
    public class Dish : IReviewable, IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TypeOfDish TypeOfDish { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
        public float Rating { get; set; }
         
        public Dish(int id, string name, string description, decimal price, TypeOfDish TypeOfDish, int stock, List<Review> reviews, float rating )
        {
            Id = id;
            Name = name;
            Description = description;
            this.TypeOfDish = TypeOfDish;
            Price = price;
            Reviews = reviews;
            Rating = rating;
        }

        public void AddReview(Review review)
        {
            Reviews.Add(review);
            Console.WriteLine($"Review added for {Name}: {review.Text}");
        }
    }
    public class DishRepository : Repository<Dish> { }
}