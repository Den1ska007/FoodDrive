// Models/ViewModels/CreateReviewViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace FoodDrive.Models.ViewModels
{
    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "Оберіть користувача")]
        public int SelectedUserId { get; set; }

        [Required(ErrorMessage = "Оберіть страву")]
        public int SelectedDishId { get; set; }

        [Required(ErrorMessage = "Вкажіть оцінку")]
        [Range(1, 5, ErrorMessage = "Оцінка має бути від 1 до 5")]
        public byte Rating { get; set; }

        [Required(ErrorMessage = "Введіть текст відгуку")]
        public string Text { get; set; }

        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> Dishes { get; set; } = new();
    }
}
