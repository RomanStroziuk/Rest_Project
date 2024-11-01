using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Warehouses.Exceptions;
using Domain.Warehouses;
using MediatR;

namespace Application.Warehouses.Commands;

public class DeleteWarehouseCommand : IRequest<Result<Warehouse, WarehouseException>>
{
    public required Guid Id { get; init; }
}

public class DeleteWarehouseCommandHandler(IWarehouseRepository warehouseRepository) : IRequestHandler<DeleteWarehouseCommand, Result<Warehouse, WarehouseException>>
{
    public async Task<Result<Warehouse, WarehouseException>> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouseId = new WarehouseId(request.Id);
        var warehouse = await warehouseRepository.GetById(warehouseId, cancellationToken);

        return await warehouse.Match<Task<Result<Warehouse, WarehouseException>>>(
            async w => await Deleteentity(w, cancellationToken),
            () => Task.FromResult<Result<Warehouse, WarehouseException>>(new WarehouseNotFoundException(warehouseId)));
    }

    private async Task<Result<Warehouse, WarehouseException>> Deleteentity(Warehouse warehouse, CancellationToken cancellationToken)
    {
        try
        {
            return await warehouseRepository.Delete(warehouse, cancellationToken);
        }
        catch (Exception e)
        {
            return new WarehouseUnknownException(warehouse.Id, e);
        }
    }
}
