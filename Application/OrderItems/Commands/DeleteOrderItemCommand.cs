using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using MediatR;

namespace Application.OrderItems.Commands;

public class DeleteOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required Guid Id { get; init; }
}
public class DeleteOrderItemCommandHandler(IOrderItemRepository orderItemRepository)
    : IRequestHandler<DeleteOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(DeleteOrderItemCommand request, CancellationToken cancellationToken)
    {
        var orderItemId = new OrderItemId(request.Id);
        
        var orderItem = await orderItemRepository.GetById(orderItemId, cancellationToken);
        return await orderItem.Match<Task<Result<OrderItem, OrderItemException>>>(
            async o => 
            {
                return await DeleteEntity(o, cancellationToken);
            }, () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemNotFoundException(orderItemId)));
    }

    private async Task<Result<OrderItem, OrderItemException>> DeleteEntity(
        OrderItem orderItem,
        CancellationToken cancellationToken)
    {
        try
        {
            return await orderItemRepository.Delete(orderItem, cancellationToken);
        }
        catch (Exception e)
        {
            return new OrderItemUnknownException(orderItem.Id, e);
        }
    }
}