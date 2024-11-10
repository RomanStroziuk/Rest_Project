using Domain.Orders;


namespace Api.Dtos;

public record OrderDto(
    Guid? Id,
    DateTime OrderDate,
    int  TotalPrice,
    Guid UserId,
    UserDto? User,
    Guid StatusId,
    StatusDto? Status,
    List<OrderItemDto> OrderItems)// Assuming OrderItemDto is defined elsewhere


{
    public static OrderDto FromDomainModel(Order order)
        =>  new OrderDto(
                Id: order.Id.Value,
                OrderDate: order.OrderDate,
                TotalPrice: order.TotalPrice,
                UserId: order.UserId.Value,
                User: order.User != null ? UserDto.FromDomainModel(order.User) : null,
                StatusId: order.StatusId.Value,
                Status: order.Status != null ? StatusDto.FromDomainModel(order.Status) : null, // Convert Status to StatusDto
                OrderItems: order.OrderItems.ConvertAll(OrderItemDto.FromDomainModel) // Convert OrderItems to List<OrderItemDto>
            );
 
}