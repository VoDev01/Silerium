namespace Silerium.DTO
{
    public class ProductsFiltersDTO
    {
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string SortOrder { get; set; }
        public int CurrentPage { get; set; } = 1;
    }
}
