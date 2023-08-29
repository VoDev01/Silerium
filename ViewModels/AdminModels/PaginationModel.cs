namespace Silerium.ViewModels.AdminModels
{
    public class PaginationModel
    {
        public int Page { get; set; }
        public int TotalCount { get; set; }
        public int FirstPageIndex { get; set; }
        public int LastPageIndex { get; set;}
    }
}
