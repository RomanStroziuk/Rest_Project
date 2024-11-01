using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.SneakerWarehouses.Exceptions;
using Domain.SneakerWarehouses;
using MediatR;

namespace Application.SneakerWarehouses.Commands;

public record UpdateSneakerQuantityInWarehouseCommand : IRequest<Result<SneakerWarehouse, SneakerWarehouseException>>
{
    public required Guid Id { get; init; }

    public required int Quantity { get; init; }
}
public class UpdateSneakerQuantityInWarehouseCommandHandler(ISneakerWarehouseRepository sneakerWarehouseRepository) 
    : IRequestHandler<UpdateSneakerQuantityInWarehouseCommand, Result<SneakerWarehouse, SneakerWarehouseException>>
{
    public async Task<Result<SneakerWarehouse, SneakerWarehouseException>> Handle(UpdateSneakerQuantityInWarehouseCommand request, CancellationToken cancellationToken)
    {
        var sneakerWarehouseId = new SneakerWarehouseId(request.Id);

        var sneakerWarehouse = await sneakerWarehouseRepository.GetById(sneakerWarehouseId, cancellationToken);

        return await sneakerWarehouse.Match<Task<Result<SneakerWarehouse, SneakerWarehouseException>>>(
            async s =>
            {
                return await UpdateSneakerQuantityInWarehouse(s, request.Quantity, cancellationToken);
            },
            () => Task.FromResult<Result<SneakerWarehouse, SneakerWarehouseException>>(
                new SneakerWarehouseNotFoundException(sneakerWarehouseId)));
    }

    private async Task<Result<SneakerWarehouse, SneakerWarehouseException>> UpdateSneakerQuantityInWarehouse(
        SneakerWarehouse sneakerWarehouse,
        int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            sneakerWarehouse.UpdateSneakerQuantity(quantity);
            return await sneakerWarehouseRepository.Update(sneakerWarehouse, cancellationToken);
        }
        catch (Exception e)
        {
            return new SneakerWarehouseUnknownException(sneakerWarehouse.Id, e);
        }
    }
}