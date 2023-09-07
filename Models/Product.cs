using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Silerium.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        public float PriceRub { get; set; }
        public int StockAmount { get; set; }
        public bool Available { get; set; }
        public Subcategory Subcategory { get; set; }
        public List<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public Page Page { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
