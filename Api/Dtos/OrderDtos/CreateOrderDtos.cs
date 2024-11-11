using Domain.Orders;


namespace Api.Dtos;

public record CreateOrderDto(
    DateTime OrderDate,
    Guid UserId,
    Guid StatusId)
    
{
    public static CreateOrderDto FromDomainModel(Order order)
        =>  new CreateOrderDto(
            OrderDate: order.OrderDate,
            UserId: order.UserId.Value,
            StatusId: order.StatusId.Value
        );
 
}