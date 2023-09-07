using Silerium.Models;

namespace Silerium.ViewModels.AdminModels
{
    public class AdminCategoryViewModel
    {
        public List<Category> Categories { get; set; }
        public PaginationModel PaginationModel { get; set; }
    }
}
