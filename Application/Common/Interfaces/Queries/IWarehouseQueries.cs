using Domain.Warehouses;

namespace Application.Common.Interfaces.Queries;

public interface IWarehouseQueries
{
    Task<IReadOnlyList<Warehouse>> GetAll(CancellationToken cancellationToken);
}