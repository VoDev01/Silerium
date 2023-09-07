using Microsoft.AspNetCore.Authorization;

namespace Silerium.PermissionAuth
{
    internal class PermissionRequirment : IAuthorizationRequirement
    {
        public string Permission { get; private set; }
        public PermissionRequirment(string permission)
        {
            Permission = permission;
        }
    }
}
