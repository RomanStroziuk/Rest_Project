using Domain.OrderItems;

namespace Application.Common.Interfaces.Queries;

public interface IOrderItemQueries
{
    Task<IReadOnlyList<OrderItem>> GetAll(CancellationToken cancellationToken);
}