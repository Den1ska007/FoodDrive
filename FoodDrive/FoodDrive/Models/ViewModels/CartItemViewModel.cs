using FoodDrive.Entities;

namespace FoodDrive.Models.ViewModels
{
    public class CartItemViewModel
    {
        public Dish Dish { get; set; }
        public int Quantity { get; set; }
    }
}
