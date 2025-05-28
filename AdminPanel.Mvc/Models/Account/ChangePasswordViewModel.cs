using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Mvc.Models.Account;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Текущий пароль обязателен")]
    [MinLength(6)]
    [MaxLength(25)]
    [Display(Name = "Текущий пароль")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Новый пароль обязателен")]
    [DataType(DataType.Password)]
    [MinLength(6)]
    [MaxLength(25)]
    [Display(Name = "Новый пароль")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Подтвердите новый пароль")]
    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; }
}
