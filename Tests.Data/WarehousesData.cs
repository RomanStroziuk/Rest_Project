using Domain.Warehouses;

namespace Tests.Data;

public class WarehousesData
{
    public static Warehouse MainWarehouse => Warehouse.New(WarehouseId.New(), "Main location", 20);
}