﻿using Domain.Sneakers;
using Domain.Brands;
using Domain.Сategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SneakerConfiguration : IEntityTypeConfiguration<Sneaker>
{
    public void Configure(EntityTypeBuilder<Sneaker> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new SneakerId(x));

        builder.Property(x => x.Model).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Size).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Price).IsRequired().HasColumnType("varchar(255)");

        builder.HasOne(x => x.Brand)
            .WithMany()
            .HasForeignKey(x => x.BrandId)
            .HasConstraintName("fk_users_brands_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("fk_users_categories_id")
            .OnDelete(DeleteBehavior.Restrict);
        
      
    }
}