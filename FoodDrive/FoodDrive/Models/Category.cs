using FoodDrive.Interfaces;

namespace FoodDrive.Models
{ 
    public class Category : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class CategoryRepository : Repository<Category> { }
}