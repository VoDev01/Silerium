namespace Silerium.Models
{
    public class ProductsCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CatalogueModel Catalogue { get; set; }
    }
}
