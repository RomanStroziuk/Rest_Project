using Domain.OrderItems;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task<Option<OrderItem>> GetById(OrderItemId orderItemIdId, CancellationToken cancellationToken);
    Task<OrderItem> Create(OrderItem orderItem, CancellationToken cancellationToken);
    Task<OrderItem> Update(OrderItem orderItem, CancellationToken cancellationToken);
    Task<OrderItem> Delete(OrderItem orderItem, CancellationToken cancellationToken);
}