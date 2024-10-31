using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Warehouses.Exceptions;
using Domain.Warehouses;
using MediatR;
using Optional;

namespace Application.Warehouses.Commands;

public class UpdateWarehouseCommand : IRequest<Result<Warehouse, WarehouseException>>
{
    public required Guid Id { get; init; }
    public required string Location { get; init; }
    public required int TotalQuantity { get; init; }
}
public class UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository) : IRequestHandler<UpdateWarehouseCommand, Result<Warehouse, WarehouseException>>
{
    public async Task<Result<Warehouse, WarehouseException>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouseId = new WarehouseId(request.Id);
        
        var warehouse = await warehouseRepository.GetById(warehouseId, cancellationToken);
        
        return await warehouse.Match(
            async w =>
            {
                var existingWarehouse = await CheckDuplicated(warehouseId, request.Location, cancellationToken);
                
                return await existingWarehouse.Match(
                    ef => Task.FromResult<Result<Warehouse, WarehouseException>>(new WarehouseAlreadyExistsException(ef.Id)),
                    async () => await UpdateEntity(w, request.Location, request.TotalQuantity, cancellationToken));
            },
            () => Task.FromResult<Result<Warehouse, WarehouseException>>(new WarehouseNotFoundException(warehouseId)));
    }

    private async Task<Result<Warehouse, WarehouseException>> UpdateEntity(Warehouse warehouse,
        string location,
        int totalQuantity,
        CancellationToken cancellationToken)
    {
        try
        {
            warehouse.UpdateDetails(location, totalQuantity);
            return await warehouseRepository.Update(warehouse, cancellationToken);
        }
        catch (Exception e)
        {
            return new WarehouseUnknownException(warehouse.Id, e);
        }
    }

    private async Task<Option<Warehouse>> CheckDuplicated(
        WarehouseId warehouseId,
        string location,
        CancellationToken cancellationToken)
    {
        var warehouse = await warehouseRepository.SearchByLocation(location, cancellationToken);

        return warehouse.Match(
            w => w.Id == warehouseId ? Option.None<Warehouse>() : Option.Some(w),
            Option.None<Warehouse>);
    }
}