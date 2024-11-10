using Domain.Sneakers;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface ISneakerRepository
{
    Task<Sneaker> Add(Sneaker sneaker, CancellationToken cancellationToken);
    Task<Sneaker> Update(Sneaker sneaker, CancellationToken cancellationToken);
    Task<Sneaker> Delete(Sneaker sneaker, CancellationToken cancellationToken);
    
    Task<Option<Sneaker>> SearchByName(string name, CancellationToken cancellationToken);

    Task<Option<Sneaker>> GetById(SneakerId id, CancellationToken cancellationToken);
    
}