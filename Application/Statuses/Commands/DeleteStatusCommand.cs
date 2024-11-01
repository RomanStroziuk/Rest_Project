using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;

namespace Application.Statuses.Commands;

public class DeleteStatusCommand : IRequest<Result<Status, StatusExceptions>>
{
    public required Guid Id { get; init; }
}
public class DeleteStatusCommandHandler(IStatusRepository statusRepository) : IRequestHandler<DeleteStatusCommand, Result<Status, StatusExceptions>>
{
    public async Task<Result<Status, StatusExceptions>> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
    {
        var statusId = new StatusId(request.Id);
        var existingStatus = await statusRepository.GetById(statusId, cancellationToken);

        return await existingStatus.Match<Task<Result<Status, StatusExceptions>>>(
            async s => await DeleteEntity(s, cancellationToken),
            () => Task.FromResult<Result<Status, StatusExceptions>>(new StatusNotFoundException(statusId)));
    }

    private async Task<Result<Status, StatusExceptions>> DeleteEntity(Status status, CancellationToken cancellationToken)
    {
        try
        {
            return await statusRepository.Delete(status, cancellationToken);
        }
        catch (Exception e)
        {
            return new StatusUnknownException(status.Id, e);
        }
    }
}