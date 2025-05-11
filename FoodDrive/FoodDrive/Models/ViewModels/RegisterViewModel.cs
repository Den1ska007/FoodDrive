// Models/ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Обов'язкове поле")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ім'я має бути 3-50 символів")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Обов'язкове поле")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Пароль має бути від 8 символів")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Обов'язкове поле")]
    public string Address { get; set; }
    [Required(ErrorMessage = "Баланс обов'язковий")]
    [Range(0, double.MaxValue, ErrorMessage = "Баланс не може бути від'ємним")]
    public decimal Balance { get; set; }
    [Required(ErrorMessage = "Оберіть роль")]
    public string Role { get; set; } = "Customer";
}