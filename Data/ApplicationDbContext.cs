using Microsoft.EntityFrameworkCore;
using Silerium.Models;

namespace Silerium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) {  }
        public ApplicationDbContext(string connectionString) : base(GetOptions(connectionString))
        { }
        public DbSet<CategoryModel> CatalogueModels { get; set; }
        public DbSet<ProductImageModel> ProductImages { get; set; }
        public DbSet<ProductModel> ProductModels { get; set; }
        public DbSet<ProductsCategoryModel> ProductCategories { get; set; }
        public DbSet<ProductsStockModel> ProductStockModels { get; set; }
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
