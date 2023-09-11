using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }
        [MaxLength(30)]
        public string? Surname { get; set; }
        [MaxLength(75)]
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(75)]
        public string? City { get; set; }
        [MaxLength(200)]
        public string? HomeAdress { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsOnline { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public List<Role> Roles { get; set; }
    }
}
