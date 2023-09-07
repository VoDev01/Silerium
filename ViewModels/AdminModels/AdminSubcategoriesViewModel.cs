using Silerium.Models;

namespace Silerium.ViewModels.AdminModels
{
    public class AdminSubcategoriesViewModel
    {
        public List<Subcategory> Subcategories { get; set; }
        public PaginationModel PaginationModel { get; set; }
    }
}
