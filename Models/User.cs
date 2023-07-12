
using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        [Required]
        public string NickName { get; set; }
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
        public DateTime? BirthDate { get; set; }
        [MaxLength(50)]
        [Required]
        public string Country { get; set; }
        [MaxLength(200)]
        public string? HomeAdress { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
