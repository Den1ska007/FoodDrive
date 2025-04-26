using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace FoodDrive.Models.ViewModels
{
    public class EditReviewViewModel
    {
        public int Id { get; set; }

        [Required]
        public int SelectedUserId { get; set; }
        public List<SelectListItem> Users { get; set; } = new();

        [Required]
        public int SelectedDishId { get; set; }
        public List<SelectListItem> Dishes { get; set; } = new();

        [Required]
        [Range(1, 5)]
        public byte Rating { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
