using Domain.SneakerWarehouses;

namespace Application.Common.Interfaces.Queries;

public interface ISneakerWarehouseQueries
{
    Task<IReadOnlyList<SneakerWarehouse>> GetAll(CancellationToken cancellationToken);
}