using Domain.Warehouses;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IWarehouseRepository
{
    Task<Option<Warehouse>> GetById(WarehouseId warehouseId, CancellationToken cancellationToken);
    Task<Option<Warehouse>> SearchByLocation(string location, CancellationToken cancellationToken);
    Task<Warehouse> Create(Warehouse warehouse, CancellationToken cancellationToken);
    Task<Warehouse> Update(Warehouse warehouse, CancellationToken cancellationToken);
    Task<Warehouse> Delete(Warehouse warehouse, CancellationToken cancellationToken);
}