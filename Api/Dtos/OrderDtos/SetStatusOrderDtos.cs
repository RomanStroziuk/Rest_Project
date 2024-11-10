using Domain.Orders;


namespace Api.Dtos;

public record SetStatusOrderDto(  
      Guid OrderId ,
      Guid StatusId)
    
{
    public static SetStatusOrderDto FromDomainModel(Order order)
        =>  new SetStatusOrderDto(
            OrderId: order.Id.Value,
            StatusId: order.StatusId.Value
        );
 
}