using Domain.Statuses;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IStatusRepository
{
    Task<Option<Status>> GetById(StatusId statusId, CancellationToken cancellationToken);
    Task<Option<Status>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Status> Delete(Status status, CancellationToken cancellationToken);
    Task<Status> Create(Status status, CancellationToken cancellationToken);
    Task<Status> Update(Status status, CancellationToken cancellationToken);
    
    Task<Option<Status>> GetByTitle(string title, CancellationToken cancellationToken);

}