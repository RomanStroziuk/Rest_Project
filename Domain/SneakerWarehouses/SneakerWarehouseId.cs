namespace Domain.SneakerWarehouses;

public record SneakerWarehouseId(Guid Value)
{
    public static SneakerWarehouseId New() => new(Guid.NewGuid());
    public static SneakerWarehouseId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}