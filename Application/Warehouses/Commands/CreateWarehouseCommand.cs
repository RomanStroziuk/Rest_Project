using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Warehouses.Exceptions;
using Domain.Warehouses;
using MediatR;

namespace Application.Warehouses.Commands;

public class CreateWarehouseCommand : IRequest<Result<Warehouse, WarehouseException>>
{
    public required string Location { get; init; }
    public required int TotalQuantity { get; init; }
}

public class CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository) : IRequestHandler<CreateWarehouseCommand, Result<Warehouse, WarehouseException>>
{
    public async Task<Result<Warehouse, WarehouseException>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var existingWarehouse = await warehouseRepository.SearchByLocation(request.Location, cancellationToken);

        return await existingWarehouse.Match(
            r => Task.FromResult<Result<Warehouse, WarehouseException>>(new WarehouseAlreadyExistsException(r.Id)),
            async () => await CreateEntity(request.Location, request.TotalQuantity, cancellationToken));
    }
    private async Task<Result<Warehouse, WarehouseException>> CreateEntity(
        string location,
        int totalQuantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Warehouse.New(WarehouseId.New(), location, totalQuantity);
            return await warehouseRepository.Create(entity, cancellationToken);
        }
        catch (Exception e)
        {
           return new WarehouseUnknownException(WarehouseId.Empty(), e);
        }
    }
}