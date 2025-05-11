using System.ComponentModel.DataAnnotations;

namespace FoodDrive.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Обов'язкове поле")]
        [Display(Name = "Ім'я")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обов'язкове поле")]
        [Display(Name = "Адреса")]
        public string Address { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути від 6 символів")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердіть новий пароль")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }
    }
}