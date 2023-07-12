using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class ProductsSpecificationRepository : RepositoryBase<ProductSpecification>, IProductSpecification
    {
        public ProductsSpecificationRepository(DbContext db) : base(db)
        {
        }
    }
}
