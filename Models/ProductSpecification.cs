using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class ProductSpecification
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Укажите название хотя-бы одной характеристики")]
        public string Name { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Укажите значение характеристики")]
        public string Specification { get; set; }
        public Product Product { get; set; }
    }
}
