namespace Domain.Brands;

public record BrandId(Guid Value)
{
    public static BrandId New() => new(Guid.NewGuid());
    public static BrandId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}