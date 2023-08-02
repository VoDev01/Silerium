namespace Silerium.Models.Query
{
    //public enum SortOrder { POP_DESC, POP_ASC, PRICE_DESC, PRICE_ASC, NAME_DESC, NAME_ASC}
    public class ProductsQuery
    {
        public int Page { get; set; } = 1;
        public string? CategoryName { get; set; }
        public string? SubcategoryName { get; set; }
        public string? ProductName { get; set; }
        //public SortOrder SortOrder { get; set; } = SortOrder.NAME_DESC;
    }
}
