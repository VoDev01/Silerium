using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class ProductsRepository : RepositoryBase<Product>, IProducts
    {
        public ProductsRepository(DbContext db) : base(db)
        {
        }
    }
}
