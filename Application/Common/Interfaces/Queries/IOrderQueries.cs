using Domain.Orders;

namespace Application.Common.Interfaces.Queries;

public interface IOrderQueries
{
    Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken);
}