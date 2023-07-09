namespace Silerium.Models
{
    public class CatalogueModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ProductsCategoryModel> Categories { get; set; }
        public byte[] Image;
    }
}
