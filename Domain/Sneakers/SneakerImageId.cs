namespace Domain.Sneakers;

public record SneakerImageId(Guid Value)
{
    public static SneakerImageId Empty => new(Guid.Empty);
    public static SneakerImageId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}