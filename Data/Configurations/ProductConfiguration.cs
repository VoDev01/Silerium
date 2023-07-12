using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Silerium.Models;

namespace Silerium.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder
                .HasMany(p => p.Users)
                .WithMany(u => u.Products)
                .UsingEntity<Order>(
                    o => o
                    .HasOne(o => o.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(o => o.UserId),
                    o => o
                    .HasOne(o => o.Product)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.ProductId),
                    o =>
                    {
                        o.Property(o => o.OrderDate).HasDefaultValue(DateTime.Now);
                        o.Property(o => o.OrderAmount).HasDefaultValue(1);
                        o.Property(o => o.OrderAddress).HasDefaultValue("г.Караганда ул.Пушкина д.Колотушкина 105");
                        o.HasKey(o => new { o.UserId, o.ProductId });
                        o.ToTable("Orders");
                    }
                );
        }
    }
}
