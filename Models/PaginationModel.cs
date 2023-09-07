namespace Silerium.Models
{
    public class PaginationModel
    {
        public PaginationModel(string ActionName, string ControllerName)
        {
            FirstPageIndex = 1;
            LastPageIndex = 1;
            CurrentPage = 1;
            this.ActionName = ActionName;
            this.ControllerName = ControllerName;
        }
        public int FirstPageIndex { get; set; }
        public int LastPageIndex { get; set; }
        public int CurrentPage { get; set; }
        public int PageMultiplier { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string RouteParameters { get; set; }
        public static int MaxItemsAtPage { get; set; } = 10;
        public static int PagesCount { get; set; } = 5;
    }
}
