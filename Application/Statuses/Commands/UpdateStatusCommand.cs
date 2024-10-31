using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;
using Optional;

namespace Application.Statuses.Commands;

public class UpdateStatusCommand : IRequest<Result<Status, StatusExceptions>>
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
}

public class UpdateStatusCommandHandler(IStatusRepository statusRepository) : IRequestHandler<UpdateStatusCommand, Result<Status, StatusExceptions>>
{
    public async Task<Result<Status, StatusExceptions>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var statusId = new StatusId(request.Id);
        
        var status = await statusRepository.GetById(statusId, cancellationToken);

        return await status.Match(
            async s  => 
        {
            var existingStatus = await CheckDuplicated(statusId, request.Title, cancellationToken);
            
            return await existingStatus.Match(
                s => Task.FromResult<Result<Status, StatusExceptions>>(new StatusAlreadyExistsException(s.Id)),
                async () => await UpdateEntity(s, request.Title, cancellationToken));
        },
            () => Task.FromResult<Result<Status, StatusExceptions>>(new StatusNotFoundException(statusId)));
    }

    private async Task<Result<Status, StatusExceptions>> UpdateEntity(
        Status status,
        string title,
        CancellationToken cancellationToken)
    {
        try
        {
            status.ChangeTitle(title);
            return await statusRepository.Update(status, cancellationToken);
        }
        catch (Exception e)
        {
            return new StatusUnknownException(status.Id, e);
        }
    }

    private async Task<Option<Status>> CheckDuplicated(
        StatusId statusId,
        string name,
        CancellationToken cancellationToken)
    {
        var status = await statusRepository.SearchByName(name, cancellationToken);

        return status.Match(
            f => f.Id == statusId ? Option.None<Status>() : Option.Some(f),
            Option.None<Status>);
    }
}