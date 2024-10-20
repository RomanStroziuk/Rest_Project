using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Sneakers.Exceptions;
using Domain.Sneakers;
using MediatR;

namespace Application.Sneakers.Commands;

public record UpdateSneakerCommand : IRequest<Result<Sneaker, SneakerException>>
{
    public required Guid SneakerId { get; init; }
    public required string Model { get; init; }
    public required int Size { get; init; }
    public required int Price { get; init; }

    
}

public class UpdateSneakerCommandHandler(ISneakerRepository sneakerRepository) : IRequestHandler<UpdateSneakerCommand, Result<Sneaker, SneakerException>>
{
    public async Task<Result<Sneaker, SneakerException>> Handle(UpdateSneakerCommand request, CancellationToken cancellationToken)
    {
        var sneakerId = new SneakerId(request.SneakerId);

        var existingSneaker = await sneakerRepository.GetById(sneakerId, cancellationToken);

        return await existingSneaker.Match(
            async s => await UpdateEntity(s, request.Model, request.Size, request.Price, cancellationToken),
            () => Task.FromResult<Result<Sneaker, SneakerException>>(new SneakerNotFoundException(sneakerId)));
    }

    private async Task<Result<Sneaker, SneakerException>> UpdateEntity(
        Sneaker entity,
        string model,
        int size,
        int price,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(model, size, price);

            return await sneakerRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new SneakerUnknownException(entity.Id, exception);
        }
    }
}