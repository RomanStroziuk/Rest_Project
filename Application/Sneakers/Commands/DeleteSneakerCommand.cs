using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Sneakers.Exceptions;
using Domain.Sneakers;
using MediatR;

namespace Application.Sneakers.Commands;

public record DeleteSneakerCommand : IRequest<Result<Sneaker, SneakerException>>
{
    public required Guid SneakerId { get; init; }
}

public class DeleteSneakerCommandHandler(ISneakerRepository sneakerRepository)
    : IRequestHandler<DeleteSneakerCommand, Result<Sneaker, SneakerException>>
{
    public async Task<Result<Sneaker, SneakerException>> Handle(
        DeleteSneakerCommand request,
        CancellationToken cancellationToken)
    {
        var sneakerId = new SneakerId(request.SneakerId);

        var existingSneaker = await sneakerRepository.GetById(sneakerId, cancellationToken);

        return await existingSneaker.Match<Task<Result<Sneaker, SneakerException>>>(
            async s => await DeleteEntity(s, cancellationToken),
            () => Task.FromResult<Result<Sneaker, SneakerException>>(new SneakerNotFoundException(sneakerId)));
    }

    public async Task<Result<Sneaker, SneakerException>> DeleteEntity(Sneaker sneaker, CancellationToken cancellationToken)
    {
        try
        {
            return await sneakerRepository.Delete(sneaker, cancellationToken);
        }
        catch (Exception exception)
        {
            return new SneakerUnknownException(sneaker.Id, exception);
        }
    }
}