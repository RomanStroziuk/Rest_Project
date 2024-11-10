namespace Application.Common.Interfaces.Repositories;
using Domain.Sneakers;

public interface ISneakerImageRepository
{
    Task<SneakerImage> AddAsync(SneakerImage sneakerImage, CancellationToken cancellationToken);
    Task<SneakerImage> GetByIdAsync(SneakerImageId id, CancellationToken cancellationToken);
    Task<IEnumerable<SneakerImage>> GetBySneakerIdAsync(SneakerId sneakerId, CancellationToken cancellationToken);
    
    Task<bool> DeleteAsync(SneakerImageId id, CancellationToken cancellationToken); // Видалення
    Task<SneakerImage> UpdateAsync(SneakerImage sneakerImage, CancellationToken cancellationToken); // Оновлення
}
