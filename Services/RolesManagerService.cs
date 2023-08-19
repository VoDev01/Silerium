using Microsoft.CodeAnalysis.CSharp.Syntax;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.ViewModels;
using System.Reflection;
using System.Security.Claims;

namespace Silerium.Services
{
    public class RolesManagerService
    {
        public static List<string> GeneratePermissionsForModel(string modelName)
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
        public static void GetPermissions(List<RoleClaimViewModel> roleClaimVM, Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static| BindingFlags.Public);
            foreach(var fieldInfo in fields) 
            {
                roleClaimVM.Add(new RoleClaimViewModel { Value = fieldInfo.GetValue(null).ToString(), Type = "Permission" });
            }
        }
        public static async Task AddPermissionClaim(ClaimsIdentity user, string permission)
        {
            var userClaims = user.Claims;
            if(!userClaims.Any(u => u.Type == "Permission" && u.Value == permission))
            {
                user.AddClaim(new Claim("Permission", permission));
            }
        }
    }
}
