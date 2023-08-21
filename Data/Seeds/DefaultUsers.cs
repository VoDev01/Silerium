using Microsoft.EntityFrameworkCore;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Services;
using System.Security.Claims;

namespace Silerium.Data.Seeds
{
    public static class DefaultUsers
    {
        public static async Task<List<Claim>?> GenerateUserClaims(string userEmail, IUsers users, ILogger? logger)
        {
            if (users.IfAny(u => u.Email == userEmail))
            {
                List<Claim> userClaims = new List<Claim>
                {
                    new Claim("Name", userEmail),
                    new Claim("Role", Roles.User.ToString())
                };
                return userClaims;
            }
            else
            {
                logger?.LogError($"No user with {userEmail} email was found in database.");
                return null;
            }
        }
        public static async Task<List<Claim>?> GenerateSuperAdminClaims(string superAdminEmail, IUsers users, ILogger? logger)
        {
            if (users.IfAny(u => u.Email == superAdminEmail))
            {
                List<Claim> superAdminClaims = new List<Claim>
                {
                    new Claim("Name", superAdminEmail),
                    new Claim("Role", Roles.SuperAdmin.ToString()),
                    new Claim("Role", Roles.Admin.ToString()),
                    new Claim("Role", Roles.Moderator.ToString()),
                    new Claim("Role", Roles.Manager.ToString()),
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
                logger?.LogError($"No SuperAdmin with {superAdminEmail} email was found in database.");
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
                Name = Roles.SuperAdmin,
                Permissions = permissions
            };
            return role;
        }

        public static Role SeedAdminRole()
        {
            Role role = new Role
            {
                Id = 2,
                Name = Roles.Admin,
            };
            return role;
        }
        public static Role SeedModeratorRole()
        {
            Role role = new Role
            {
                Id = 3,
                Name = Roles.Moderator,
            };
            return role;
        }
        public static Role SeedManagerRole()
        {
            Role role = new Role
            {
                Id = 4,
                Name = Roles.Manager,
            };
            return role;
        }
        public static Role SeedUserRole()
        {
            Role role = new Role
            {
                Id = 5,
                Name = Roles.User,
            };
            return role;
        }
        public static IEnumerable<string> SeedPermissions()
        {
            var allModelsPermissions = RolesManagerService.GeneratePermissionsForModel("Product")
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
