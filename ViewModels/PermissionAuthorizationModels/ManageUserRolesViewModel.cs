namespace Silerium.ViewModels.PermissionAuthorizationModels
{
    public class ManageUserRolesViewModel
    {
        public string UserName { get; set; }
        public List<UserRolesViewModel> RolesVM { get; set; }
    }
}
