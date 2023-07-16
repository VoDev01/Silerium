using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        public string PageName { get; set; }
        public byte[] Image { get; set; }
        public IEnumerable<Subcategory> Subcategories { get; set; }
    }
}
