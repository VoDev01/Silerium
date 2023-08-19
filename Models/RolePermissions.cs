namespace Silerium.Models
{
    public class RolePermissions
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        public bool Granted { get; set; }
        public string GrantedByUser { get; set; }
    }
}
