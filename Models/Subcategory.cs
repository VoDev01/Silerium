using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Silerium.Models
{
    public class Subcategory
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        public Category Category { get; set; }
        public string? Image { get; set; }
        public IEnumerable<Product> Products { get; set;}
    }
}
