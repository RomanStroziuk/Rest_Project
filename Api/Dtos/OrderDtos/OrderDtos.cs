using Api.Dtos.OrderItemDtos;
using Api.Dtos.StatusDtos;
using Api.Dtos.UserDtos;
using Domain.Orders;

namespace Api.Dtos.OrderDtos;

public record OrderDto(
    Guid? Id,
    DateTime OrderDate,
    Guid UserId,
    UserDto? User,
    Guid StatusId,
    StatusDto? Status)


{
    public static OrderDto FromDomainModel(Order order)
        => new OrderDto(
            Id: order.Id.Value,
            OrderDate: order.OrderDate,
            UserId: order.UserId.Value,
            User: order.User != null ? UserDto.FromDomainModel(order.User) : null,
            StatusId: order.StatusId.Value,
            Status: order.Status != null ? StatusDto.FromDomainModel(order.Status) : null
        );
}