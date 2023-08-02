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
        public string Email { get; set; }
        [MaxLength(50)]
        [Required]
        public string Password { get; set; }
        [MaxLength(50)]
        [Required]
        public string ConfirmPassword { get; set; }
        public DateTime? BirthDate { get; set; }
        [MaxLength(50)]
        [Required]
        public string Country { get; set; }
        [MaxLength(200)]
        public string? HomeAdress { get; set; }
        [MaxLength(20)]
        [RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
        public string? Phone { get; set; }
        public IFormFile? PfpFile { get; set; }
    }
}
