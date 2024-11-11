using Domain.Orders;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IOrderQueries
{
    Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken);
    
    Task<Option<Order>> GetById(OrderId id, CancellationToken cancellationToken);

}