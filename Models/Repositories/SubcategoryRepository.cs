using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class SubcategoryRepository : RepositoryBase<Subcategory>, ISubcategory
    {
        public SubcategoryRepository(DbContext db) : base(db)
        {
        }
    }
}
