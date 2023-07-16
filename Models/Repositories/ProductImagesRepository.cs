using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class ProductImagesRepository : RepositoryBase<ProductImage>, IProductImages
    {
        public ProductImagesRepository(DbContext db) : base(db)
        {
        }
    }
}
