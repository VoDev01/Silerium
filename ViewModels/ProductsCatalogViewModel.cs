using Silerium.Models;

namespace Silerium.ViewModels
{
    public class ProductsCatalogViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public Subcategory Subcategory { get; set; }
        public int Page { get; set; } = 1;
        public int FirstPaginationIndex { get; set; } = 1;
        public int LastPaginationIndex { get; set; } = 20;
        public int ProductsAtPage { get; set; } = 20;
    }
}
