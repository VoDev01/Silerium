using Silerium.Models;

namespace Silerium.ViewModels.AdminModels
{
    public class AdminProductsViewModel
    {
        public List<Product> Products { get; set; }
        public PaginationModel PaginationModel { get; set; }
    }
}
