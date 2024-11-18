﻿// <auto-generated />
using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241113181218_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Brands.Brand", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_brands");

                    b.ToTable("brands", (string)null);
                });

            modelBuilder.Entity("Domain.OrderItems.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.Property<Guid>("SneakerWarehouseId")
                        .HasColumnType("uuid")
                        .HasColumnName("sneaker_warehouse_id");

                    b.HasKey("Id")
                        .HasName("pk_order_items");

                    b.HasIndex("OrderId")
                        .HasDatabaseName("ix_order_items_order_id");

                    b.HasIndex("SneakerWarehouseId")
                        .HasDatabaseName("ix_order_items_sneaker_warehouse_id");

                    b.ToTable("order_items", (string)null);
                });

            modelBuilder.Entity("Domain.Orders.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("OrderDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("order_date")
                        .HasDefaultValueSql("timezone('utc', now())");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uuid")
                        .HasColumnName("status_id");

                    b.Property<int>("TotalPrice")
                        .HasColumnType("integer")
                        .HasColumnName("total_price");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.HasIndex("StatusId")
                        .HasDatabaseName("ix_orders_status_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_orders_user_id");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("Domain.Roles.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.ToTable("roles", (string)null);
                });

            modelBuilder.Entity("Domain.SneakerWarehouses.SneakerWarehouse", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("SneakerId")
                        .HasColumnType("uuid")
                        .HasColumnName("sneaker_id");

                    b.Property<int>("SneakerQuantity")
                        .HasColumnType("integer")
                        .HasColumnName("sneaker_quantity");

                    b.Property<Guid>("WarehouseId")
                        .HasColumnType("uuid")
                        .HasColumnName("warehouse_id");

                    b.HasKey("Id")
                        .HasName("pk_sneaker_warehouses");

                    b.HasIndex("SneakerId")
                        .HasDatabaseName("ix_sneaker_warehouses_sneaker_id");

                    b.HasIndex("WarehouseId")
                        .HasDatabaseName("ix_sneaker_warehouses_warehouse_id");

                    b.ToTable("sneaker_warehouses", (string)null);
                });

            modelBuilder.Entity("Domain.Sneakers.Sneaker", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("BrandId")
                        .HasColumnType("uuid")
                        .HasColumnName("brand_id");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("category_id");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("model");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("price");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("size");

                    b.HasKey("Id")
                        .HasName("pk_sneakers");

                    b.HasIndex("BrandId")
                        .HasDatabaseName("ix_sneakers_brand_id");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_sneakers_category_id");

                    b.ToTable("sneakers", (string)null);
                });

            modelBuilder.Entity("Domain.Sneakers.SneakerImage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("S3Path")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("s3path");

                    b.Property<Guid>("SneakerId")
                        .HasColumnType("uuid")
                        .HasColumnName("sneaker_id");

                    b.HasKey("Id")
                        .HasName("pk_sneaker_images");

                    b.HasIndex("SneakerId")
                        .HasDatabaseName("ix_sneaker_images_sneaker_id");

                    b.ToTable("sneaker_images", (string)null);
                });

            modelBuilder.Entity("Domain.Statuses.Status", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_statuses");

                    b.ToTable("statuses", (string)null);
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_users_role_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Domain.Warehouses.Warehouse", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("location");

                    b.Property<int>("TotalQuantity")
                        .HasColumnType("integer")
                        .HasColumnName("total_quantity");

                    b.HasKey("Id")
                        .HasName("pk_warehouses");

                    b.ToTable("warehouses", (string)null);
                });

            modelBuilder.Entity("Domain.Сategories.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("Domain.OrderItems.OrderItem", b =>
                {
                    b.HasOne("Domain.Orders.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_orderItem_order");

                    b.HasOne("Domain.SneakerWarehouses.SneakerWarehouse", "SneakerWarehouse")
                        .WithMany("OrderItems")
                        .HasForeignKey("SneakerWarehouseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_SneakerWarehouse_OrderItems");

                    b.Navigation("Order");

                    b.Navigation("SneakerWarehouse");
                });

            modelBuilder.Entity("Domain.Orders.Order", b =>
                {
                    b.HasOne("Domain.Statuses.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Order_Status");

                    b.HasOne("Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Order_User");

                    b.Navigation("Status");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.SneakerWarehouses.SneakerWarehouse", b =>
                {
                    b.HasOne("Domain.Sneakers.Sneaker", "Sneaker")
                        .WithMany("SneakerWarehouses")
                        .HasForeignKey("SneakerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_sneakers_warehouse");

                    b.HasOne("Domain.Warehouses.Warehouse", "Warehouse")
                        .WithMany("SneakerWarehouses")
                        .HasForeignKey("WarehouseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_warehouse_sneaker");

                    b.Navigation("Sneaker");

                    b.Navigation("Warehouse");
                });

            modelBuilder.Entity("Domain.Sneakers.Sneaker", b =>
                {
                    b.HasOne("Domain.Brands.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_sneakers_brands_id");

                    b.HasOne("Domain.Сategories.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_sneakers_categories_id");

                    b.Navigation("Brand");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Domain.Sneakers.SneakerImage", b =>
                {
                    b.HasOne("Domain.Sneakers.Sneaker", "Sneaker")
                        .WithMany("Images")
                        .HasForeignKey("SneakerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_sneaker_images_id");

                    b.Navigation("Sneaker");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.HasOne("Domain.Roles.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_role");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Domain.Orders.Order", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Domain.SneakerWarehouses.SneakerWarehouse", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Domain.Sneakers.Sneaker", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("SneakerWarehouses");
                });

            modelBuilder.Entity("Domain.Warehouses.Warehouse", b =>
                {
                    b.Navigation("SneakerWarehouses");
                });
#pragma warning restore 612, 618
        }
    }
}
