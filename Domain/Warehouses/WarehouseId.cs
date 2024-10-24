namespace Domain.Warehouses;

public record WarehouseId(Guid Value)
{
    public static WarehouseId New() => new(Guid.NewGuid());
    public static WarehouseId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}