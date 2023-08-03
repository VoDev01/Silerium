using Silerium.Models;
using System.ComponentModel.DataAnnotations;

namespace Silerium.ViewModels.AuthModels
{
    public class UserLoginViewModel
    {
        [MaxLength(75)]
        [Required]
        public string Email { get; set; }
        [MaxLength(50)]
        [Required]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
