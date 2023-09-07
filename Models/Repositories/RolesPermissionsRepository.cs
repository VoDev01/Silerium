using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class RolesPermissionsRepository : RepositoryBase<RolePermissions>, IRolesPermissions
    {
        public RolesPermissionsRepository(DbContext db) : base(db)
        {
        }
    }
}
