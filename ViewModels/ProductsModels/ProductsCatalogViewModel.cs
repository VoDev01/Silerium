using Silerium.Models;

namespace Silerium.ViewModels.ProductsModels
{
    public enum SortOrder { POP_DESC, POP_ASC, PRICE_DESC, PRICE_ASC, NAME_DESC, NAME_ASC }
    public class ProductsCatalogViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Subcategory> Subcategories { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        //public Subcategory Subcategory { get; set; }
        //public ProductsQuery ProductsQuery { get; set; }
        public string SubcategoryName { get; set; }
        public SortOrder SortOrder { get; set; } = SortOrder.NAME_DESC;
        public int FirstPaginationIndex { get; set; } = 1;
        public int LastPaginationIndex { get; set; } = 20;
        public int ProductsAtPage { get; set; } = 20;
        public int CurrentPage { get; set; } = 1;
        public bool NewInstance { get; set; } = true;
    }
}
