using Domain.Statuses;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IStatusQueries
{
    Task<IReadOnlyList<Status>> GetAll(CancellationToken cancellationToken);
    Task<Option<Status>> GetByTitle(string title, CancellationToken cancellationToken);

}