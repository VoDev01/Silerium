using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class PermissionsRepository : RepositoryBase<Permission>, IPermissions
    {
        public PermissionsRepository(DbContext db) : base(db)
        {
        }
    }
}
