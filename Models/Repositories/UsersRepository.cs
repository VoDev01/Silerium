using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class UsersRepository : RepositoryBase<User>, IUsers
    {
        public UsersRepository(DbContext db) : base(db)
        {
        }
    }
}
