using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class Order
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int OrderAmount { get; set; }
        public DateTime OrderDate { get; set; }
        [MaxLength(200)]
        [Required]
        public string OrderAddress { get; set; }
    }
}
