using System.ComponentModel;
using Domain.Orders;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new OrderId(x));
        
        builder.Property(x => x.OrderDate)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.TotalPrice)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_Order_User")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .HasConstraintName("FK_Order_Status")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.OrderItems)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .HasConstraintName("FK_Order_OrderItems")
            .OnDelete(DeleteBehavior.Restrict);
    }
}