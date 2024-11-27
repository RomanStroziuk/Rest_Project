using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Sneakers;
using Domain.SneakerWarehouses;
using MediatR;

namespace Application.OrderItems.Commands;

public class CreateOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required Guid OrderId { get; init; }
    public required Guid SneakerWarehouseId { get; init; }
    public required int Quantity { get; init; }
}

public class CreateOrderItemCommandHandler(
    IOrderRepository orderRepository,
    ISneakerWarehouseRepository sneakerWarehouseRepository,
    IOrderItemRepository orderItemRepository) 
    : IRequestHandler<CreateOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(CreateOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);
        var sneakerWarehouseId = new SneakerWarehouseId(request.SneakerWarehouseId);
        
        var order = await orderRepository.GetById(orderId, cancellationToken);
        var sneaker = await sneakerWarehouseRepository.GetById(sneakerWarehouseId, cancellationToken);
        
        return await order.Match<Task<Result<OrderItem, OrderItemException>>>(
            async o =>
        {
            return await sneaker.Match<Task<Result<OrderItem, OrderItemException>>>(
                async s =>
            {
                if (request.Quantity > s.SneakerQuantity)
                {
                    return new NotEnoughSneakerException(sneakerWarehouseId);
                }
                return await CreateEntity(s.Id, o.Id, request.Quantity, s.Sneaker.Price,cancellationToken);
            }, () => Task.FromResult<Result<OrderItem, OrderItemException>>(new SneakerWarehouseNotFoundException(sneakerWarehouseId)));
        }, () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderNotFoundException(orderId)));
    }
    
    private async Task<Result<OrderItem, OrderItemException>> CreateEntity(
        SneakerWarehouseId sneakerId,
        OrderId orderId,
        int quantity,
        int price,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = OrderItem.New(OrderItemId.New(), sneakerId, orderId, quantity, 0);
            entity.CalculateTotalPrice(price);
            return await orderItemRepository.Create(entity, cancellationToken);
        }
        catch (Exception e)
        {
            return new OrderItemUnknownException(OrderItemId.Empty(), e);
        }
    }
}