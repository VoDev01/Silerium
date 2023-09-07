using Silerium.Models;

namespace Silerium.ViewModels.AdminModels
{
    public class AdminRolesViewModel
    {
        public List<Role> Roles { get; set; }
        public PaginationModel PaginationModel { get; set; }
    }
}
