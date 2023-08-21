using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public enum Roles
    {
        SuperAdmin,
        Admin,
        Moderator,
        Manager,
        User
    }
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public Roles Name { get; set; } = Roles.User;
        public IEnumerable<Permission>? Permissions { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<RolePermissions> RolePermissions { get; set; }
    }
}
