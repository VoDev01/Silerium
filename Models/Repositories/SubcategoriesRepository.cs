using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class SubcategoriesRepository : RepositoryBase<Subcategory>, ISubcategories
    {
        public SubcategoriesRepository(DbContext db) : base(db)
        {
        }
    }
}
