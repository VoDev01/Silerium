using Silerium.ViewModels.PermissionAuthorizationModels;
using System.Reflection;
using System.Security.Claims;

namespace Silerium.Services.RolesSerivces
{
    public enum PermissionType { View, Edit, Create, Delete, DownloadData }
    public class RolesManagerService
    {
        public static List<string> GeneratePermissionsNamesForModel(string modelName)
        {
            return new List<string>
            {
                $"Permission.{modelName}.View",
                $"Permission.{modelName}.Create",
                $"Permission.{modelName}.Edit",
                $"Permission.{modelName}.Delete",
                $"Permission.{modelName}.DownloadData"
            };
        }
        public static string GeneratePermissionNameForModel(string modelName, PermissionType permissionType)
        {
            return $"Permission.{modelName}.{permissionType}";
        }
        public static void GetPermissions(List<RoleClaimViewModel> roleClaimVM, Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                roleClaimVM.Add(new RoleClaimViewModel { Value = fieldInfo.GetValue(null).ToString(), Type = "Permission" });
            }
        }

        public static async Task AddPermissionClaim(ClaimsIdentity user, string permission)
        {
            var userClaims = user.Claims;
            if (!userClaims.Any(u => u.Type == "Permission" && u.Value == permission))
            {
                user.AddClaim(new Claim("Permission", permission));
            }
        }
        public static async Task AddRole(ClaimsIdentity user, string role)
        {
            var userClaims = user.Claims;
            if (!userClaims.Any(u => u.Type == "Role" && u.Value == role))
            {
                user.AddClaim(new Claim("Role", role));
            }
        }
    }
}
