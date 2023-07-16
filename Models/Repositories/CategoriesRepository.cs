using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class CategoriesRepository : RepositoryBase<Category>, ICategories
    {
        public CategoriesRepository(DbContext db) : base(db)
        {
        }
    }
}
