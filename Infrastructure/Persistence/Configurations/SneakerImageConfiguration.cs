using Domain.Sneakers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Converters;

namespace Infrastructure.Persistence.Configurations;

public class SneakerImageConfiguration : IEntityTypeConfiguration<SneakerImage>
{
    public void Configure(EntityTypeBuilder<SneakerImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new SneakerImageId(x));

        builder.Property(x => x.SneakerId)
            .HasConversion(x => x.Value, x => new SneakerId(x));
        
        // Конфігурація властивості для S3Path
        builder.Property(x => x.S3Path)
            .IsRequired()  // Вказуємо, що шлях до зображення є обов'язковим
            .HasMaxLength(500);  // Можна обмежити довжину шляху

        
        
        builder.HasOne(x => x.Sneaker)
            .WithMany(s => s.Images) // Додайте навігаційну властивість тут
            .HasForeignKey(x => x.SneakerId)
            .HasConstraintName("fk_sneaker_images_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

