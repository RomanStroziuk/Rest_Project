using Domain.SneakerWarehouses;

namespace Api.Dtos.SneakerWarehouseDtos;

public record SneakerWarehouseDto(
    Guid? Id,
    Guid SneakerId,
    SneakerDto? Sneaker,
    Guid WarehouseId,
    WarehouseDto? Warehouse,
    int SneakerQuantity)

{
    public static SneakerWarehouseDto FromDomainModel(SneakerWarehouse sneakerWarehouse)
        => new(
            Id: sneakerWarehouse.Id.Value,
            SneakerId: sneakerWarehouse.SneakerId.Value,
            Sneaker: sneakerWarehouse.Sneaker != null ? SneakerDto.FromDomainModel(sneakerWarehouse.Sneaker) : null,
            WarehouseId: sneakerWarehouse.WarehouseId.Value,
            Warehouse: sneakerWarehouse.Warehouse != null ? WarehouseDto.FromDomainModel(sneakerWarehouse.Warehouse) : null,
            SneakerQuantity: sneakerWarehouse.SneakerQuantity
        );
}