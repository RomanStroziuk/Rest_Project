using Domain.OrderItems;
using Domain.Sneakers;
using Domain.Warehouses;

namespace Domain.SneakerWarehouses;

public class SneakerWarehouse
{
    public SneakerWarehouseId Id { get; }
    public Sneaker? Sneaker { get; }
    public SneakerId SneakerId { get; }
    public Warehouse? Warehouse { get; }
    public WarehouseId WarehouseId { get; }
    public int SneakerQuantity { get; private set; }
    public List<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    private SneakerWarehouse(SneakerWarehouseId id, SneakerId sneakerId, WarehouseId warehouseId,
        int sneakerQuantity)
    {
        Id = id;
        SneakerId = sneakerId;
        WarehouseId = warehouseId;
        SneakerQuantity = sneakerQuantity;
    }
    public static SneakerWarehouse New(SneakerWarehouseId id, SneakerId sneakerId, WarehouseId warehouseId, int sneakerQuantity)
    => new(id, sneakerId, warehouseId, sneakerQuantity);
}




