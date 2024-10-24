using Domain.OrderItems;
using Domain.Orders;
using Domain.SneakerWarehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new OrderItemId(x));

        builder.Property(x => x.Quantity).IsRequired();

        builder.Property(x => x.OrderId).HasConversion(x => x.Value, x => new OrderId(x));
        builder.Property(x => x.SneakerWarehouseId).HasConversion(x => x.Value, x => new SneakerWarehouseId(x));
        
        builder.HasOne(x => x.SneakerWarehouse)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.SneakerWarehouseId)
            .HasConstraintName("fk_orderItem_sneakerWarehouse")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.OrderId)
            .HasConstraintName("fk_orderItem_order")
            .OnDelete(DeleteBehavior.Restrict);
    }
}