using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Interface;
using RepositoryPattern.Repository;
using Silerium.Models.Interfaces;

namespace Silerium.Models.Repositories
{
    public class OrdersRepository : RepositoryBase<Order>, IOrders
    {
        public OrdersRepository(DbContext db) : base(db)
        {
        }
    }
}
