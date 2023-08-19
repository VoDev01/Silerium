using Microsoft.EntityFrameworkCore;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Services;
using System.Security.Claims;

namespace Silerium.Data.Seeds
{
    public static class DefaultUsers
    {
        public static async Task<List<Claim>> GenerateSuperAdminClaims(string superAdminEmail, IUsers users, ILogger? logger)
        {
            if (users.IfAny(u => u.Email == superAdminEmail))
            {
                List<Claim> superAdminClaims = new List<Claim>
                {
                    new Claim("Name", superAdminEmail),
                    new Claim("Role", Roles.SuperAdmin.ToString()),
                    new Claim("Role", Roles.Admin.ToString()),
                    new Claim("Role", Roles.Moderator.ToString()),
                    new Claim("Role", Roles.User.ToString())
                };
                foreach (var permission in SeedPermissions())
                {
                    superAdminClaims.Add(new Claim("Permission", permission));
                }
                return superAdminClaims;
            }
            else
            {
                logger?.LogError($"No superadmin with {superAdminEmail} was found in database.");
                return new List<Claim>();
            }
        }
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
