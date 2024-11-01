using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.SneakerWarehouses.Exceptions;
using Domain.Sneakers;
using Domain.SneakerWarehouses;
using Domain.Warehouses;
using MediatR;

namespace Application.SneakerWarehouses.Commands;

public record DeleteSneakerFromWarehouseCommand : IRequest<Result<SneakerWarehouse, SneakerWarehouseException>>
{
    public required Guid Id { get; init; }
}

public class DeleteSneakerFromWarehouseCommandHandler(
    ISneakerWarehouseRepository sneakerWarehouseRepository) : IRequestHandler<DeleteSneakerFromWarehouseCommand, Result<SneakerWarehouse, SneakerWarehouseException>>
{
    public async Task<Result<SneakerWarehouse, SneakerWarehouseException>> Handle(DeleteSneakerFromWarehouseCommand request, CancellationToken cancellationToken)
    {
        var sneakerWarehouseId = new SneakerWarehouseId(request.Id);

        var sneakerWarehouse = await sneakerWarehouseRepository.GetById(sneakerWarehouseId, cancellationToken);

        return await sneakerWarehouse.Match<Task<Result<SneakerWarehouse, SneakerWarehouseException>>>(
            async s =>
            {
                return await RemoveSneakerWarehouse(s, cancellationToken);
            },
            () => Task.FromResult<Result<SneakerWarehouse, SneakerWarehouseException>>(
                new SneakerWarehouseNotFoundException(sneakerWarehouseId)));
    }

    private async Task<Result<SneakerWarehouse, SneakerWarehouseException>> RemoveSneakerWarehouse(
        SneakerWarehouse sneakerWarehouse,
        CancellationToken cancellationToken)
    {
        try
        {
            return await sneakerWarehouseRepository.Delete(sneakerWarehouse, cancellationToken);
        }
        catch (Exception e)
        {
            return new SneakerWarehouseUnknownException(SneakerWarehouseId.Empty(), e);
        }
    }
}