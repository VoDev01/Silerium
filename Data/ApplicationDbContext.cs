using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Silerium.Data.Configurations;
using Silerium.Data.Convertions;
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
        public DbSet<Request> Requests { get; set; }

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
            List<string> permissionsNames = DefaultUsers.SeedPermissions().ToList();
            for (int i = 0; i < permissionsNames.Count; i++)
            {
                permissions.Add(new Permission { Id = i+1, PermissionName = permissionsNames[i] });
            }
            modelBuilder.Entity<Permission>().HasData(permissions);

            Role superAdminRole = DefaultUsers.SeedSuperAdminRole();
            permissions = superAdminRole.Permissions.ToList();
            superAdminRole.Permissions = null;
            modelBuilder.Entity<Role>().HasData(superAdminRole);

            modelBuilder.Entity<Role>().HasData(DefaultUsers.SeedAdminRole());
            modelBuilder.Entity<Role>().HasData(DefaultUsers.SeedModeratorRole());
            modelBuilder.Entity<Role>().HasData(DefaultUsers.SeedManagerRole());
            modelBuilder.Entity<Role>().HasData(DefaultUsers.SeedUserRole());

            List<RolePermissions> rolePermissions = new List<RolePermissions>();
            for (int i = 0; i < permissions.Count; i++)
            {
                rolePermissions.Add(new RolePermissions { RoleId = superAdminRole.Id, PermissionId = permissions[i].Id, Granted = true, GrantedByUser = "Dev" });
            }
            modelBuilder.Entity<RolePermissions>().HasData(rolePermissions);
        }
    }
}
