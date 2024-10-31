using Domain.Warehouses;


public record WarehouseDto(
    Guid? Id,
    string Location,
    int TotalQuantity,
    List<SneakerWarehouseDto> SneakerWarehouses) // Припускаючи, що SneakerWarehouseDto вже визначено

{
    public static WarehouseDto FromDomainModel(Warehouse warehouse)
        => new(
            Id: warehouse.Id.Value,
            Location: warehouse.Location,
            TotalQuantity: warehouse.TotalQuantity,
            SneakerWarehouses: warehouse.SneakerWarehouses.ConvertAll(SneakerWarehouseDto.FromDomainModel) // Перетворення SneakerWarehouses
        );
}