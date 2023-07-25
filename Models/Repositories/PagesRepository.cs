using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class PagesRepository : RepositoryBase<Page>, IPages
    {
        public PagesRepository(DbContext db) : base(db)
        {
        }
    }
}
