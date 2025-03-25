namespace FoodDrive.Interfaces
{
    public interface IEntity
    {
        int id { get; set; }
        private static int _latestId = 0;
    }
}
