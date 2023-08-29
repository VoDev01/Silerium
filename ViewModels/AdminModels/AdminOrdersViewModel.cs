using Silerium.Models;

namespace Silerium.ViewModels.AdminModels
{
    public class AdminOrdersViewModel
    {
        public List<Order> Orders { get; set; }
        public PaginationModel PaginationModel { get; set; }
    }
}
