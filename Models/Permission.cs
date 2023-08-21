using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<RolePermissions> RolePermissions { get; set; }
    }
}
