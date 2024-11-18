using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.SneakerWarehouses;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class SneakerWarehouseRepository(ApplicationDbContext context) : ISneakerWarehouseQueries, ISneakerWarehouseRepository
{
    public async Task<IReadOnlyList<SneakerWarehouse>> GetAll(CancellationToken cancellationToken)
    {
        return await context.SneakerWarehouses
            .Include(x => x.Sneaker)
            .Include(x => x.Warehouse)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<SneakerWarehouse>> GetById(SneakerWarehouseId sneakerWarehouseId, CancellationToken cancellationToken)
    {
        var entity = await context.SneakerWarehouses
            .Include(x => x.Sneaker)
            .Include(x => x.Warehouse)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == sneakerWarehouseId, cancellationToken);
        
        return entity == null ? Option.None<SneakerWarehouse>() : Option.Some(entity);
    }

    public async Task<SneakerWarehouse> Create(SneakerWarehouse sneakerWarehouse, CancellationToken cancellationToken)
    {
        await context.SneakerWarehouses.AddAsync(sneakerWarehouse, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return await context.SneakerWarehouses
            .Include(x => x.Sneaker)
            .Include(x => x.Warehouse)
            .AsNoTracking()
            .FirstAsync(x => x.Id == sneakerWarehouse.Id, cancellationToken);
    }

    public async Task<SneakerWarehouse> Update(SneakerWarehouse sneakerWarehouse, CancellationToken cancellationToken)
    {
        context.SneakerWarehouses.Update(sneakerWarehouse);
        await context.SaveChangesAsync(cancellationToken);
        return sneakerWarehouse;
    }

    public async Task<SneakerWarehouse> Delete(SneakerWarehouse sneakerWarehouse, CancellationToken cancellationToken)
    {
        context.SneakerWarehouses.Remove(sneakerWarehouse);
        await context.SaveChangesAsync(cancellationToken);
        return sneakerWarehouse;
    }
}