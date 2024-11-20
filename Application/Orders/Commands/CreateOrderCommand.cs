using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Orders.Exceptions;
using Domain.Orders;
using Domain.Users;
using Domain.Statuses;
using MediatR;

namespace Application.Orders.Commands;

public record CreateOrderCommand : IRequest<Result<Order, OrderException>>
{
    public required DateTime OrderDate  {get; init;}
    public required Guid UserId { get; init; }
    public required Guid StatusId { get; init; }
    
}

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IStatusRepository statusRepository)
    : IRequestHandler<CreateOrderCommand, Result<Order, OrderException>>
{
    public async Task<Result<Order, OrderException>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var user = await userRepository.GetById(userId, cancellationToken);

        var statusId = new StatusId(request.StatusId);
        var status = await statusRepository.GetById(statusId, cancellationToken);

        return await user.Match<Task<Result<Order, OrderException>>>(async u =>
            {
                return await status.Match<Task<Result<Order, OrderException>>>(async c =>
                    {
                        return await CreateEntity(request.OrderDate, userId, statusId, cancellationToken);
                    },
                    () => Task.FromResult<Result<Order, OrderException>>(new OrderStatusNotFoundException(statusId)));
            },
            () => Task.FromResult<Result<Order, OrderException>>(new OrderUserNotFoundException(userId)));
    }

    private async Task<Result<Order, OrderException>> CreateEntity(
        DateTime orderDate,
        UserId userId,
        StatusId statusId,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Order.New(OrderId.New(), userId, statusId);
            return await orderRepository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderUnknownException(OrderId.Empty(), exception);
        }
    }
}
