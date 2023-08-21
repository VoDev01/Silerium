using Silerium.Models;

namespace Silerium.ViewModels.PermissionAuthorizationModels
{
    public class PermissionViewModel
    {
        public int RoleId { get; set; }
        public List<RoleClaimViewModel> RoleClaims { get; set; }
    }
}
