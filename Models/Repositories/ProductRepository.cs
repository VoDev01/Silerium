using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProduct
    {
        public ProductRepository(DbContext db) : base(db)
        {
        }
    }
}
