namespace Silerium.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Specification> Specifications { get; set; }
        public List<ProductImageModel> Images { get; set; }
        public IEnumerable<ProductsStockModel> ProductsStockModel { get; set; }
    }
    public class Specification
    {
        public string Name;
        private string Value { get; set; }
    }
}
