using System.ComponentModel.DataAnnotations;

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
        public int PriceRub { get; set; }
        public int StockAmount { get; set; }
        public bool Available { get; set; }
        public Subcategory Subcategory { get; set; }
        public IEnumerable<ProductSpecification> Specifications { get; set; }
        public IEnumerable<ProductImage> Images { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
