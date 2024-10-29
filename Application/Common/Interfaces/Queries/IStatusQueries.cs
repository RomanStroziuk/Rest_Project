using Domain.Statuses;

namespace Application.Common.Interfaces.Queries;

public interface IStatusQueries
{
    Task<IReadOnlyList<Status>> GetAll(CancellationToken cancellationToken);
}