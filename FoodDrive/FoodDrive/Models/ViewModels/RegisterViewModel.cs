// Models/ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Обов'язкове поле")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Обов'язкове поле")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Обов'язкове поле")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Оберіть роль")]
    public string Role { get; set; } = "Customer";
}