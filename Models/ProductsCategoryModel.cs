namespace Silerium.Models
{
    public class ProductsCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryModel Catalogue { get; set; }
    }
}
