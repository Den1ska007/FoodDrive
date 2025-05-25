using System.ComponentModel.DataAnnotations;

namespace FoodDrive.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Обов'язкове поле")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обов'язкове поле")]
        public string Address { get; set; }

        // Поточний пароль обов'язковий ТІЛЬКИ при зміні пароля
        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль")]
        public string? CurrentPassword { get; set; } // Nullable

        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        public string? NewPassword { get; set; } // Nullable

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердіть новий пароль")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string? ConfirmPassword { get; set; } // Nullable
    }
}