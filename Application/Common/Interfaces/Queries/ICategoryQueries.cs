using Domain.Сategories;

namespace Application.Common.Interfaces.Queries;

public interface ICategoryQueries
{
    Task<IReadOnlyList<Category>> GetAll(CancellationToken cancellationToken);

}