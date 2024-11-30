using Domain.Sneakers;
using Domain.SneakerWarehouses;
using Domain.Warehouses;

namespace Tests.Data;

public static class SneakerWarehousesData
{
    public static SneakerWarehouse MainSneakerWarehouse(SneakerId sneakerId, WarehouseId warehouseId)
        => SneakerWarehouse.New(SneakerWarehouseId.New(),sneakerId, warehouseId, 10);
}