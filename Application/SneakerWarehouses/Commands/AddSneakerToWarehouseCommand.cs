using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.SneakerWarehouses.Exceptions;
using Domain.Sneakers;
using Domain.SneakerWarehouses;
using Domain.Warehouses;
using MediatR;

namespace Application.SneakerWarehouses.Commands;

public record AddSneakerToWarehouseCommand : IRequest<Result<SneakerWarehouse, SneakerWarehouseException>>
{
    public required Guid SneakerId { get; set; }
    public required Guid WarehouseId { get; set; }
    public required int Quantity { get; set; }
}
public class AddSneakerToWarehouseCommandHandler(
    ISneakerWarehouseRepository sneakerWarehouseRepository,
    ISneakerRepository sneakerRepository,
    IWarehouseRepository warehouseRepository) 
    : IRequestHandler<AddSneakerToWarehouseCommand, Result<SneakerWarehouse, SneakerWarehouseException>>
{
    public async Task<Result<SneakerWarehouse, SneakerWarehouseException>> Handle(AddSneakerToWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        var sneakerId = new SneakerId(request.SneakerId);
        var warehouseId = new WarehouseId(request.WarehouseId);

        var sneaker = await sneakerRepository.GetById(sneakerId, cancellationToken);
        var warehouse = await warehouseRepository.GetById(warehouseId, cancellationToken);

        return await sneaker.Match<Task<Result<SneakerWarehouse, SneakerWarehouseException>>>(
                async s =>
                {
                    return await warehouse.Match<Task<Result<SneakerWarehouse, SneakerWarehouseException>>>(
                        async w =>
                        {
                            if (request.Quantity > w.TotalQuantity)
                            {
                                return new InsufficientStockException(warehouseId, w.TotalQuantity, request.Quantity);
                            }

                            return await AddSneakerToWarehouse(s.Id, w.Id, request.Quantity, cancellationToken);
                        },
                        () => Task.FromResult<Result<SneakerWarehouse, SneakerWarehouseException>>(
                            new WarehouseNotFoundException(warehouseId))
                    );
                },
                () => Task.FromResult<Result<SneakerWarehouse, SneakerWarehouseException>>(
                    new SneakerNotFoundException(sneakerId)));
    }

    private async Task<Result<SneakerWarehouse, SneakerWarehouseException>> AddSneakerToWarehouse(
        SneakerId sneakerId,
        WarehouseId warehouseId,
        int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = SneakerWarehouse.New(SneakerWarehouseId.New(), sneakerId, warehouseId, quantity);
            return await sneakerWarehouseRepository.Create(entity, cancellationToken);
        }
        catch (Exception e)
        {
            return new SneakerWarehouseUnknownException(SneakerWarehouseId.Empty(), e);
        }
    }
}