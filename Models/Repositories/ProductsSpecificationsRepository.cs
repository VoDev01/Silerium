using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class ProductsSpecificationsRepository : RepositoryBase<ProductSpecification>, IProductSpecifications
    {
        public ProductsSpecificationsRepository(DbContext db) : base(db)
        {
        }
    }
}
