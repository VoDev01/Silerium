using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Silerium.Data.Configurations;
using Silerium.Data.Seeds;
using Silerium.Models;
using Silerium.Services;

namespace Silerium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public ApplicationDbContext(string connectionString) : base(GetOptions(connectionString))
        { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSpecification> ProductSpecification { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Page> Pages { get; set; }

        private IEntityTypeConfiguration<Product> productConfiguration = new ProductConfiguration();
        private IEntityTypeConfiguration<Role> roleConfiguration = new RoleConfiguration();
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>()
                .HaveColumnType("date");
            configurationBuilder.Properties<OrderStatus>()
                .HaveConversion<EnumConverter<OrderStatus>>()
                .HaveColumnType("nvarchar(10)");
            configurationBuilder.Properties<Roles>()
                .HaveConversion<EnumConverter<Roles>>()
                .HaveColumnType("nvarchar(100)");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(productConfiguration);
            modelBuilder.ApplyConfiguration(roleConfiguration);

            List<Permission> permissions = new List<Permission>();
            foreach (var permission in DefaultUsers.SeedPermissions())
            {
                permissions.Add(new Permission { PermissionName = permission });
            }
            
            modelBuilder.Entity<Permission>().HasData(permissions);
            modelBuilder.Entity<Role>().HasData(DefaultUsers.SeedSuperAdminRole(Permissions));
        }
    }
}
