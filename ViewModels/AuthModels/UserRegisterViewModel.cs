using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Silerium.ViewModels.AuthModels
{
    public class UserRegisterViewModel
    {
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }
        [MaxLength(30)]
        public string? Surname { get; set; }
        [MaxLength(75)]
        [Required]
        [Remote("CheckEmail", "User", ErrorMessage = "Данный email уже кем-то используется.")]
        public string Email { get; set; }
        [MaxLength(50)]
        [Required]
        public string Password { get; set; }
        [MaxLength(50)]
        [Required]
        [Remote("CheckPasswords", "User", ErrorMessage = "Пароли должны совпадать.", AdditionalFields = "Password")]
        public string ConfirmPassword { get; set; }
        public DateTime? BirthDate { get; set; }
        [MaxLength(50)]
        [Required]
        public string Country { get; set; }
        [MaxLength(200)]
        public string? HomeAdress { get; set; }
        [MaxLength(20)]
        [Remote("CheckPhone", "User", ErrorMessage = "Телефон заполнен неверно.")]
        public string? Phone { get; set; }
        public IFormFile? PfpFile { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
