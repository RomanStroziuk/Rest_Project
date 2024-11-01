using Domain.Warehouses;

namespace Api.Dtos;

public record WarehouseDto(
    Guid? Id,
    string Location,
    int TotalQuantity)

{
    public static WarehouseDto FromDomainModel(Warehouse warehouse)
        => new(
            Id: warehouse.Id.Value,
            Location: warehouse.Location,
            TotalQuantity: warehouse.TotalQuantity
        );
}