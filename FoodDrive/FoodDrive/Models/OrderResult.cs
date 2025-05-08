namespace FoodDrive.Models
{
    public class OrderResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Order Order { get; set; }
    }
}
