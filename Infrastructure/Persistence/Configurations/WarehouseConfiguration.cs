using Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new WarehouseId(x));
        
        builder.Property(x => x.Location).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.TotalQuantity).IsRequired();    
        
        builder.HasMany(x => x.SneakerWarehouses)
            .WithOne(x => x.Warehouse)
            .HasForeignKey(x => x.WarehouseId)
            .HasConstraintName("fk_warehouse_sneaker")
            .OnDelete(DeleteBehavior.Restrict);
    }
}