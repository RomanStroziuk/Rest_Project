using Domain.Sneakers;
using Domain.SneakerWarehouses;
using Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SneakerWarehouseConfiguration : IEntityTypeConfiguration<SneakerWarehouse>
{
    public void Configure(EntityTypeBuilder<SneakerWarehouse> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new SneakerWarehouseId(x));

        builder.Property(x => x.SneakerQuantity)
            .IsRequired();
        
        builder.Property(x => x.SneakerId).HasConversion(x => x.Value, x => new SneakerId(x));
        builder.Property(x => x.WarehouseId).HasConversion(x => x.Value, x => new WarehouseId(x));
        
        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.SneakerWarehouses)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Sneaker)
            .WithMany(x => x.SneakerWarehouses)
            .HasForeignKey(x => x.SneakerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.OrderItems)
            .WithOne(x => x.SneakerWarehouse)
            .HasForeignKey(x => x.SneakerWarehouseId)
            .HasConstraintName("FK_SneakerWarehouse_OrderItems")
            .OnDelete(DeleteBehavior.Restrict);
    }
}