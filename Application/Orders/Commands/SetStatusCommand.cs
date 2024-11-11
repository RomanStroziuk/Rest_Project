using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Orders.Exceptions;
using Application.Statuses.Exceptions;
using Domain.Orders;
using Domain.Statuses;
using MediatR;

namespace Application.Orders.Commands;

public class SetStatusCommand : IRequest<Result<Order, OrderException>>
{
    public required Guid OrderId  { get; init; }
    public required Guid StatusId { get; init; }
}

public class SetStatusCommandHandler : IRequestHandler<SetStatusCommand, Result<Order, OrderException>>
{
    private readonly IOrderRepository orderRepository;
    private readonly IStatusRepository statusRepository;

    public SetStatusCommandHandler(IOrderRepository orderRepository, IStatusRepository statusRepository)
    {
        this.orderRepository = orderRepository;
        this.statusRepository = statusRepository;
    }

    public async Task<Result<Order, OrderException>> Handle(SetStatusCommand request, CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);
        var statusId = new StatusId(request.StatusId);
        var existingOrder = await orderRepository.GetById(orderId, cancellationToken);
        var existingStatus = await statusRepository.GetById(statusId, cancellationToken);

        return await existingOrder.Match(
            async order => await existingStatus.Match(
                async status => await SetStatus(order, status.Id, cancellationToken),
                () => Task.FromResult<Result<Order, OrderException>>(
                    new OrderStatusNotFoundException(statusId))),
            () => Task.FromResult<Result<Order, OrderException>>(
                new OrderNotFoundException(orderId)));
    }

    private async Task<Result<Order, OrderException>> SetStatus(Order order, StatusId statusId, CancellationToken cancellationToken)
    {
        order.SetStatus(statusId);
        return await orderRepository.Update(order, cancellationToken);
    }
    
}
