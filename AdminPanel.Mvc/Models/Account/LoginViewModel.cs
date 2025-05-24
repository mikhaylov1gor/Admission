using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Mvc.Models.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    [MinLength(6, ErrorMessage = "Минимальная длина 6 символов")]
    [MaxLength(25, ErrorMessage = "Максимальная длина 25 символов")]
    public string Password { get; set; }
}