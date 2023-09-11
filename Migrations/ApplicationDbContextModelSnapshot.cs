﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Silerium.Data;

#nullable disable

namespace Silerium.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<int>("RolesId")
                        .HasColumnType("int");

                    b.Property<int>("UsersId")
                        .HasColumnType("int");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("RoleUser", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.Order", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("OrderAddress")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasDefaultValue("г.Караганда ул.Пушкина д.Колотушкина 105");

                    b.Property<int>("OrderAmount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OrderStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.Property<float>("TotalPrice")
                        .HasColumnType("real");

                    b.HasKey("UserId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("Orders", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.Page", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Pages", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("PermissionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permissions", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PermissionName = "Permission.Product.View"
                        },
                        new
                        {
                            Id = 2,
                            PermissionName = "Permission.Product.Create"
                        },
                        new
                        {
                            Id = 3,
                            PermissionName = "Permission.Product.Edit"
                        },
                        new
                        {
                            Id = 4,
                            PermissionName = "Permission.Product.Delete"
                        },
                        new
                        {
                            Id = 5,
                            PermissionName = "Permission.Product.DownloadData"
                        },
                        new
                        {
                            Id = 6,
                            PermissionName = "Permission.Category.View"
                        },
                        new
                        {
                            Id = 7,
                            PermissionName = "Permission.Category.Create"
                        },
                        new
                        {
                            Id = 8,
                            PermissionName = "Permission.Category.Edit"
                        },
                        new
                        {
                            Id = 9,
                            PermissionName = "Permission.Category.Delete"
                        },
                        new
                        {
                            Id = 10,
                            PermissionName = "Permission.Category.DownloadData"
                        },
                        new
                        {
                            Id = 11,
                            PermissionName = "Permission.Subcategory.View"
                        },
                        new
                        {
                            Id = 12,
                            PermissionName = "Permission.Subcategory.Create"
                        },
                        new
                        {
                            Id = 13,
                            PermissionName = "Permission.Subcategory.Edit"
                        },
                        new
                        {
                            Id = 14,
                            PermissionName = "Permission.Subcategory.Delete"
                        },
                        new
                        {
                            Id = 15,
                            PermissionName = "Permission.Subcategory.DownloadData"
                        },
                        new
                        {
                            Id = 16,
                            PermissionName = "Permission.User.View"
                        },
                        new
                        {
                            Id = 17,
                            PermissionName = "Permission.User.Create"
                        },
                        new
                        {
                            Id = 18,
                            PermissionName = "Permission.User.Edit"
                        },
                        new
                        {
                            Id = 19,
                            PermissionName = "Permission.User.Delete"
                        },
                        new
                        {
                            Id = 20,
                            PermissionName = "Permission.User.DownloadData"
                        },
                        new
                        {
                            Id = 21,
                            PermissionName = "Permission.Role.View"
                        },
                        new
                        {
                            Id = 22,
                            PermissionName = "Permission.Role.Create"
                        },
                        new
                        {
                            Id = 23,
                            PermissionName = "Permission.Role.Edit"
                        },
                        new
                        {
                            Id = 24,
                            PermissionName = "Permission.Role.Delete"
                        },
                        new
                        {
                            Id = 25,
                            PermissionName = "Permission.Role.DownloadData"
                        },
                        new
                        {
                            Id = 26,
                            PermissionName = "Permission.Permission.View"
                        },
                        new
                        {
                            Id = 27,
                            PermissionName = "Permission.Permission.Create"
                        },
                        new
                        {
                            Id = 28,
                            PermissionName = "Permission.Permission.Edit"
                        },
                        new
                        {
                            Id = 29,
                            PermissionName = "Permission.Permission.Delete"
                        },
                        new
                        {
                            Id = 30,
                            PermissionName = "Permission.Permission.DownloadData"
                        },
                        new
                        {
                            Id = 31,
                            PermissionName = "Permission.Order.View"
                        },
                        new
                        {
                            Id = 32,
                            PermissionName = "Permission.Order.Create"
                        },
                        new
                        {
                            Id = 33,
                            PermissionName = "Permission.Order.Edit"
                        },
                        new
                        {
                            Id = 34,
                            PermissionName = "Permission.Order.Delete"
                        },
                        new
                        {
                            Id = 35,
                            PermissionName = "Permission.Order.DownloadData"
                        });
                });

            modelBuilder.Entity("Silerium.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Available")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("PageId")
                        .HasColumnType("int");

                    b.Property<float>("PriceRub")
                        .HasColumnType("real");

                    b.Property<int>("StockAmount")
                        .HasColumnType("int");

                    b.Property<int>("SubcategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PageId");

                    b.HasIndex("SubcategoryId");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.ProductSpecification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("Specification")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductSpecification", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "SuperAdmin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Moderator"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Manager"
                        },
                        new
                        {
                            Id = 5,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("Silerium.Models.RolePermissions", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.Property<bool>("Granted")
                        .HasColumnType("bit");

                    b.Property<string>("GrantedByUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermissions", (string)null);

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            PermissionId = 1,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 2,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 3,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 4,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 5,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 6,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 7,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 8,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 9,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 10,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 11,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 12,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 13,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 14,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 15,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 16,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 17,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 18,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 19,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 20,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 21,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 22,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 23,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 24,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 25,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 26,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 27,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 28,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 29,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 30,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 31,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 32,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 33,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 34,
                            Granted = true,
                            GrantedByUser = "Dev"
                        },
                        new
                        {
                            RoleId = 1,
                            PermissionId = 35,
                            Granted = true,
                            GrantedByUser = "Dev"
                        });
                });

            modelBuilder.Entity("Silerium.Models.Subcategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Subcategories", (string)null);
                });

            modelBuilder.Entity("Silerium.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("City")
                        .HasMaxLength(75)
                        .HasColumnType("nvarchar(75)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(75)
                        .HasColumnType("nvarchar(75)");

                    b.Property<string>("HomeAdress")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<byte[]>("ProfilePicture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Surname")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.HasOne("Silerium.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Silerium.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Silerium.Models.Order", b =>
                {
                    b.HasOne("Silerium.Models.Product", "Product")
                        .WithMany("Orders")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Silerium.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Silerium.Models.Product", b =>
                {
                    b.HasOne("Silerium.Models.Page", "Page")
                        .WithMany("Products")
                        .HasForeignKey("PageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Silerium.Models.Subcategory", "Subcategory")
                        .WithMany("Products")
                        .HasForeignKey("SubcategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Page");

                    b.Navigation("Subcategory");
                });

            modelBuilder.Entity("Silerium.Models.ProductImage", b =>
                {
                    b.HasOne("Silerium.Models.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Silerium.Models.ProductSpecification", b =>
                {
                    b.HasOne("Silerium.Models.Product", "Product")
                        .WithMany("Specifications")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Silerium.Models.RolePermissions", b =>
                {
                    b.HasOne("Silerium.Models.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Silerium.Models.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Silerium.Models.Subcategory", b =>
                {
                    b.HasOne("Silerium.Models.Category", "Category")
                        .WithMany("Subcategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Silerium.Models.Category", b =>
                {
                    b.Navigation("Subcategories");
                });

            modelBuilder.Entity("Silerium.Models.Page", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Silerium.Models.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("Silerium.Models.Product", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Orders");

                    b.Navigation("Specifications");
                });

            modelBuilder.Entity("Silerium.Models.Role", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("Silerium.Models.Subcategory", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Silerium.Models.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
