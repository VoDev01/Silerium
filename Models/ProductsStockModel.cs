namespace Silerium.Models
{
    public class ProductsStockModel
    {
        public int ProductId { get; set; }
        public int ProductAmount { get; set; }
        public ProductModel Product { get; set; }
    }
}
