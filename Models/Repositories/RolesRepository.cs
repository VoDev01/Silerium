using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class RolesRepository : RepositoryBase<Role>, IRoles
    {
        public RolesRepository(DbContext db) : base(db)
        {
        }
    }
}
