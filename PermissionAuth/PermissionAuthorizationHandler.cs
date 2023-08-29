using Microsoft.AspNetCore.Authorization;

namespace Silerium.PermissionAuth
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirment>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirment requirement)
        {
            if (context == null)
            {
                await Task.CompletedTask;
            }
            var permissions = context.User.Claims.Where(
                u => u.Type == "Permission" &&
                u.Value == requirement.Permission);
            if (permissions.Any())
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "Недостаточно прав для совершения этого действия"));
                await Task.CompletedTask;
            }
        }
    }
}
