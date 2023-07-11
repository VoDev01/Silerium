using Microsoft.EntityFrameworkCore;
using Silerium.Data.Configurations;
using Silerium.Models;

namespace Silerium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {  }
        public ApplicationDbContext(string connectionString) : base(GetOptions(connectionString))
        { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductsCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSpecification> ProductSpecification { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        private IEntityTypeConfiguration<Product> productConfiguration = new ProductConfiguration();
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(productConfiguration);
        }
    }
}
