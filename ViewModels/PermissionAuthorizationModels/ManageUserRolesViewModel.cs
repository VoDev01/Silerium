namespace Silerium.ViewModels.PermissionAuthorizationModels
{
    public class ManageUserRolesViewModel
    {
        public int UserId { get; set; }
        public List<UserRolesViewModel> RolesVM { get; set; }
    }
}
