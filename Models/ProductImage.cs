using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string Image { get; set; }
        public Product Product { get; set; }
    }
}
