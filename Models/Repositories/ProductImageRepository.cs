using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class ProductImageRepository : RepositoryBase<ProductImage>, IProductImage
    {
        public ProductImageRepository(DbContext db) : base(db)
        {
        }
    }
}
