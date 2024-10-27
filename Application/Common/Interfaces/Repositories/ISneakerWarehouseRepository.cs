using Domain.SneakerWarehouses;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface ISneakerWarehouseRepository
{
    Task<Option<SneakerWarehouse>> GetById(SneakerWarehouseId sneakerWarehouseId, CancellationToken cancellationToken);
    Task<SneakerWarehouse> Create(SneakerWarehouse sneakerWarehouse, CancellationToken cancellationToken);
    Task<SneakerWarehouse> Update(SneakerWarehouse sneakerWarehouse, CancellationToken cancellationToken);
    Task<SneakerWarehouse> Delete(SneakerWarehouse sneakerWarehouse, CancellationToken cancellationToken);
}