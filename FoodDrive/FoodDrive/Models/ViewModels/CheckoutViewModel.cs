using System.Collections.Generic;
using FoodDrive.Models;

namespace FoodDrive.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
    }
}
