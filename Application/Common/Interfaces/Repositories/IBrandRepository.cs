using Domain.Brands;
using Optional;


namespace Application.Common.Interfaces.Repositories;

public interface IBrandRepository
{
    Task<Option<Brand>> GetById(BrandId id, CancellationToken cancellationToken);
    Task<Brand> Add(Brand brand, CancellationToken cancellationToken);
    Task<Option<Brand>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Brand> Update(Brand brand, CancellationToken cancellationToken);
    Task<Brand> Delete(Brand brand, CancellationToken cancellationToken);
}