using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class Restaurant : BaseEntity
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public TypeOfDish TypeOfDish { get; set; }
        public required string Rating { get; set; }
        public required int Id { get; set; }

    }
    public class RestaurantRepository : Repository<Restaurant>
    {
        public RestaurantRepository(IDataStorage<Restaurant> storage) : base(storage)
        {
        }

    }
}
