namespace Silerium.Models
{
    public class ProductImageModel
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public ProductModel product { get; set; }
    }
}
