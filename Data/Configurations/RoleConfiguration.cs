using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Silerium.Models;

namespace Silerium.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<RolePermissions>(
                j => j
                .HasOne(rp => rp.Permission)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.Permission),
                j => j
                .HasOne(rp => rp.Role)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.RoleId),
                j =>
                {
                    j.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                    j.Property(rp => rp.Granted);
                    j.Property(rp => rp.GrantedByUser);
                    j.ToTable("RolePermissions");
                });
        }
    }
}
