using Silerium.Models;

namespace Silerium.ViewModels.ProductsModels
{
    public enum SortOrder { POP_DESC, POP_ASC, PRICE_DESC, PRICE_ASC, NAME_DESC, NAME_ASC }
    public class ProductsCatalogViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Subcategory> Subcategories { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string SubcategoryName { get; set; }
        public SortOrder SortOrder { get; set; } = SortOrder.NAME_DESC;
        public PaginationModel PaginationModel { get; set; }
        public bool NewInstance { get; set; } = true;
    }
}
