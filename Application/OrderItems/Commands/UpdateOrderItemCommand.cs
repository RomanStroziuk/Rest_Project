using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using MediatR;

namespace Application.OrderItems.Commands;

public class UpdateOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required Guid Id { get; init; }
    public required int Quantity { get; init; }
    public required int TotalPrice { get; init; }
}
public class UpdateOrderItemCommandHandler(IOrderItemRepository orderItemRepository) 
    : IRequestHandler<UpdateOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        var orderItemId = new OrderItemId(request.Id);
        var orderItem = await orderItemRepository.GetById(orderItemId, cancellationToken);
        
        return await orderItem.Match<Task<Result<OrderItem, OrderItemException>>>(
            async o => 
            {
                return await UpdateEntity(o, request.Quantity, request.TotalPrice, cancellationToken); 
            }, () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemNotFoundException(orderItemId)));
    }

    private async Task<Result<OrderItem, OrderItemException>> UpdateEntity(
        OrderItem orderItem,
        int quantity,
        int totalPrice,
        CancellationToken cancellationToken)
    {
        try
        {
            orderItem.UpdateDetails(quantity, totalPrice);
            return await orderItemRepository.Update(orderItem, cancellationToken);
        }
        catch (Exception e)
        {
            return new OrderItemUnknownException(orderItem.Id, e);
        }
    }
}