using Microsoft.EntityFrameworkCore;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Services;
using System.Security.Claims;

namespace Silerium.Data.Seeds
{
    public static class DefaultUsers
    {
        public static async Task<List<Claim?>> GenerateClaims(string email, IUsers users, ILogger? logger)
        {
            if (users.IfAny(u => u.Email == email))
            {
                User user = users.GetAllWithInclude(u => u.Roles).Where(u => u.Email == email).FirstOrDefault();
                List<Claim> claims = new List<Claim>
                {
                    new Claim("Name", email)
                };
                foreach(var role in user.Roles)
                {
                    claims.Add(new Claim("Role", role.Name));
                }
                foreach (var permission in SeedPermissions())
                {
                    claims.Add(new Claim("Permission", permission));
                }
                return claims;
            }
            else
            {
                logger?.LogWarning($"No user with {email} email was found in database.");
                return null;
            }
        }
        public static Role SeedSuperAdminRole()
        {
            List<Permission> permissions = new List<Permission>(); 
            List<string> permissionsNames = SeedPermissions().ToList();
            for (int i = 0; i < permissionsNames.Count; i++)
            {
                permissions.Add(new Permission { Id = i+1, PermissionName = permissionsNames[i] });
            }
            Role role = new Role
            {
                Id = 1,
                Name = Roles.SuperAdmin.ToString(),
                Permissions = permissions
            };
            return role;
        }

        public static Role SeedAdminRole()
        {
            Role role = new Role
            {
                Id = 2,
                Name = Roles.Admin.ToString(),
            };
            return role;
        }
        public static Role SeedModeratorRole()
        {
            Role role = new Role
            {
                Id = 3,
                Name = Roles.Moderator.ToString(),
            };
            return role;
        }
        public static Role SeedManagerRole()
        {
            Role role = new Role
            {
                Id = 4,
                Name = Roles.Manager.ToString(),
            };
            return role;
        }
        public static Role SeedUserRole()
        {
            Role role = new Role
            {
                Id = 5,
                Name = Roles.User.ToString(),
            };
            return role;
        }
        public static IEnumerable<string> SeedPermissions()
        {
            var allModelsPermissions = RolesManagerService.GeneratePermissionsNamesForModel("Product")
                .Concat(RolesManagerService.GeneratePermissionsNamesForModel("Category"))
                .Concat(RolesManagerService.GeneratePermissionsNamesForModel("Subcategory"))
                .Concat(RolesManagerService.GeneratePermissionsNamesForModel("User"))
                .Concat(RolesManagerService.GeneratePermissionsNamesForModel("Role"))
                .Concat(RolesManagerService.GeneratePermissionsNamesForModel("Permission"))
                .Concat(RolesManagerService.GeneratePermissionsNamesForModel("Order"));
            return allModelsPermissions;
        }
    }
}
