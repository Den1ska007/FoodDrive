using System.Collections.Generic;
using FoodDrive.Models;

namespace FoodDrive.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItemViewModel> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
    }
}
