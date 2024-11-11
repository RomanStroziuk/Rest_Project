using Domain.Orders;
using Optional;
using Domain.Users;
using Domain.Statuses;

namespace Application.Common.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Option<Order>> GetById(OrderId orderId, CancellationToken cancellationToken);
    Task<Order> Create(Order order, CancellationToken cancellationToken);
    Task<Order> Update(Order order, CancellationToken cancellationToken);
    Task<Order> Delete(Order order, CancellationToken cancellationToken);
    
    Task<Option<Order>> SearchByStatusAndUser(UserId userId, StatusId statusId, CancellationToken cancellationToken);

    
}