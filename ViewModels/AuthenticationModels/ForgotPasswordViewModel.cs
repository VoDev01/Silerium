using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Silerium.ViewModels.AuthenticationModels
{
    public class ForgotPasswordViewModel
    {
        [MaxLength(75)]
        [Required(ErrorMessage = "Введите Email.")]
        public string? Email { get; set; }
        [MaxLength(50)]
        [MinLength(12, ErrorMessage = "Пароль должен содержать как минимум 12 символов.")]
        [Required(ErrorMessage = "Введите пароль.")]
        public string Password { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Введите пароль повторно.")]
        [Remote(action: "CheckPasswords", controller: "User", AdditionalFields = "ConfirmPassword", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
        public bool CodeConfirm { get; set; }
        public int Code { get; set; }
    }
}
