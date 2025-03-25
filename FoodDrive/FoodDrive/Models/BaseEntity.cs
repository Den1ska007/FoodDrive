using FoodDrive.Interfaces;

namespace FoodDrive.Models
{
    public class BaseEntity : IEntity
    {
        public int id { get; set; }
        private static int _latestId = 0;
        public BaseEntity() { id = _latestId++; }
    }
}
