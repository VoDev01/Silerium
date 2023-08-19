using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
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
    }
}
