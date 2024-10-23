using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Brands;
using Microsoft.EntityFrameworkCore;
using Optional;


namespace Infrastructure.Persistence.Repositories;

public class BrandRepository(ApplicationDbContext context): IBrandRepository, IBrandQueries
{
    
    public async Task<IReadOnlyList<Brand>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Brands
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Option<Brand>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Brand>() : Option.Some(entity);
    }
    
    public async Task<Option<Brand>> GetById(BrandId id, CancellationToken cancellationToken)
    {
        var entity = await context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Brand>() : Option.Some(entity);
    }
    
    public async Task<Brand> Add(Brand brand, CancellationToken cancellationToken)
    {
        await context.Brands.AddAsync(brand, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return brand;
    }

    public async Task<Brand> Update(Brand brand, CancellationToken cancellationToken)
    {
        context.Brands.Update(brand);

        await context.SaveChangesAsync(cancellationToken);

        return brand;
    }
    
}