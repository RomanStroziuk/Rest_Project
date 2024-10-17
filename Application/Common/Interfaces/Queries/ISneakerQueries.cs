using Domain.Sneakers;
using Optional;


namespace Application.Common.Interfaces.Queries;

public interface ISneakerQueries
{
    
    Task<IReadOnlyList<Sneaker>> GetAll(CancellationToken cancellationToken);
    Task<Option<Sneaker>> GetById(SneakerId id, CancellationToken cancellationToken);
    
}