using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, ICategory
    {
        public CategoryRepository(DbContext db) : base(db)
        {
        }
    }
}
