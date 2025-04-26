// Models/ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введіть ім'я користувача")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Введіть пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

