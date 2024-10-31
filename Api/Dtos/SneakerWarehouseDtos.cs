using Domain.SneakerWarehouses;

using Api.Dtos; // Переконайтеся, що у вас є правильні простори імен

public record SneakerWarehouseDto(
    Guid? Id,
    Guid SneakerId,
    SneakerDto? Sneaker,
    Guid WarehouseId,
    WarehouseDto? Warehouse,
    int SneakerQuantity,
    List<OrderItemDto> OrderItems) // Припускаючи, що OrderItemDto вже визначено

{
    public static SneakerWarehouseDto FromDomainModel(SneakerWarehouse sneakerWarehouse)
        => new(
            Id: sneakerWarehouse.Id.Value,
            SneakerId: sneakerWarehouse.SneakerId.Value,
            Sneaker: sneakerWarehouse.Sneaker != null ? SneakerDto.FromDomainModel(sneakerWarehouse.Sneaker) : null,
            WarehouseId: sneakerWarehouse.WarehouseId.Value,
            Warehouse: sneakerWarehouse.Warehouse != null ? WarehouseDto.FromDomainModel(sneakerWarehouse.Warehouse) : null,
            SneakerQuantity: sneakerWarehouse.SneakerQuantity,
            OrderItems: sneakerWarehouse.OrderItems.ConvertAll(OrderItemDto.FromDomainModel) // Перетворення OrderItems
        );
}