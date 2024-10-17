using Domain.Brands;

namespace Application.Common.Interfaces.Queries;

public interface IBrandQueries
{
    Task<IReadOnlyList<Brand>> GetAll(CancellationToken cancellationToken);

}