using System;
using System.Collections.Generic;
using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class Dish : BaseEntity, IReviewable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TypeOfDish TypeOfDish { get; set; }
        public int Stock { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
        public float Rating { get; set; }

        public Dish()
        {
            Name = string.Empty;
            Description = string.Empty;
            TypeOfDish = TypeOfDish;
            Price = 0;
            Stock = 0;
            Rating = 0;
        }

        public void AddReview(Review review)
        {
            Reviews.Add(review);
            UpdateRating();
            Console.WriteLine($"Відгук додано для {Name}: {review.Text}");
        }

        private void UpdateRating()
        {
            if (Reviews.Any())
            {
                Rating = (float)Reviews.Average(r => r.Rating);
            }
        }
    }

    public class DishRepository : Repository<Dish>
    {
        public DishRepository(IDataStorage<Dish> storage) : base(storage)
        {
        }

        public Dish GetById(int id)
        {
            return _entities.FirstOrDefault(d => d.id == id);
        }
    }
}