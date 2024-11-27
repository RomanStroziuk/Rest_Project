using Api.Dtos.OrderDtos;
using Api.Dtos.SneakerWarehouseDtos;
using Domain.OrderItems;

namespace Api.Dtos.OrderItemDtos;

public record OrderItemDto(
    Guid? Id,
    Guid SneakerWarehouseId,
    SneakerWarehouseDto? SneakerWarehouse,
    Guid OrderId,
    OrderDto? Order,
    int Quantity,
    int  TotalPrice)


{
    public static OrderItemDto FromDomainModel(OrderItem orderItem)
        =>  new(
            Id: orderItem.Id.Value,
            SneakerWarehouseId: orderItem.SneakerWarehouseId.Value,
            SneakerWarehouse: orderItem.SneakerWarehouse != null ? SneakerWarehouseDto.FromDomainModel(orderItem.SneakerWarehouse) : null,
            OrderId: orderItem.OrderId.Value,
            Order: orderItem.Order != null ? OrderDto.FromDomainModel(orderItem.Order) : null,
            Quantity: orderItem.Quantity,
            TotalPrice: orderItem.TotalPrice
        );
 
}