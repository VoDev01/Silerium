using Silerium.Models;

namespace Silerium.ViewModels.PermissionAuthorizationModels
{
    public class PermissionViewModel
    {
        public string RoleName { get; set; }
        public List<RoleClaimViewModel> RoleClaims { get; set; }
    }
}
