using Microsoft.EntityFrameworkCore;
using Silerium.Models;
using Silerium.Services;
using System.Linq;
using System.Security.Claims;

namespace Silerium.Data.Seeds
{
    public static class DefaultUsers
    {
        public static Role SeedSuperAdminRole(DbSet<Permission> permissions)
        {
            Role role = new Role
            {
                Name = Roles.SuperAdmin,
                Permissions = permissions.ToList()
            };
            return role;
        }
        public static IEnumerable<string> SeedPermissions()
        {
            var allModelsPermissions = RolesManagerService.GeneratePermissionsForModel("Product")
                .Concat(RolesManagerService.GeneratePermissionsForModel("Product"))
                .Concat(RolesManagerService.GeneratePermissionsForModel("Category"))
                .Concat(RolesManagerService.GeneratePermissionsForModel("Subcategory"))
                .Concat(RolesManagerService.GeneratePermissionsForModel("User"))
                .Concat(RolesManagerService.GeneratePermissionsForModel("Role"))
                .Concat(RolesManagerService.GeneratePermissionsForModel("Permission"))
                .Concat(RolesManagerService.GeneratePermissionsForModel("Order"));
            return allModelsPermissions;
        }
    }
}
