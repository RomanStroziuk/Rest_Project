using Domain.SneakerWarehouses;

namespace Domain.Warehouses;

public class Warehouse
{
    public WarehouseId Id { get; }
    public string Location { get; private set; }
    public int TotalQuantity { get; private set; }
    public List<SneakerWarehouse>? SneakerWarehouses { get; private set; } = new List<SneakerWarehouse>();

    private Warehouse(WarehouseId id, string location, int totalQuantity)
    {
        Id = id;
        Location = location;
        TotalQuantity = totalQuantity;
    }

    public static Warehouse New(WarehouseId id, string location, int totalQuantity)
        => new(id, location, totalQuantity);

    public void UpdateDetails(string location, int quantity)
    {
        Location = location;
        TotalQuantity = quantity;
    }
}