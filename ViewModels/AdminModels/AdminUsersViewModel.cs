using Silerium.Models;

namespace Silerium.ViewModels.AdminModels
{
    public class AdminUsersViewModel
    {
        public List<User> Users{ get; set; }
        public PaginationModel PaginationModel { get; set; }
    }
}
