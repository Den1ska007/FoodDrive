namespace FoodDrive.Models
{
    public class Restaurant
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public TypeOfDish TypeOfDish { get; set; }
        public required string Rating { get; set; }

    }
}
