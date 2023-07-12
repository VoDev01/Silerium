using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class ProductSpecification
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [MaxLength(50)]
        [Required]
        public string Specification { get; set; }
        public Product Product { get; set; }
    }
}
