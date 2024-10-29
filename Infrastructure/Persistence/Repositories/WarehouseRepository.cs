using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class WarehouseRepository(ApplicationDbContext context) : IWarehouseRepository, IWarehouseQueries
{
    public async Task<Option<Warehouse>> GetById(WarehouseId warehouseId, CancellationToken cancellationToken)
    {
        var entity = await context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == warehouseId, cancellationToken);
        
        return entity is null ? Option.None<Warehouse>() : Option.Some(entity);
    }
    
    public async Task<IReadOnlyList<Warehouse>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Warehouses
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Option<Warehouse>> SearchByLocation(string location, CancellationToken cancellationToken)
    {
        var entity = await context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Location == location, cancellationToken);
        
        return entity is null ? Option.None<Warehouse>() : Option.Some(entity);
    }

    public async Task<Warehouse> Create(Warehouse warehouse, CancellationToken cancellationToken)
    {
        await context.Warehouses.AddAsync(warehouse, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return warehouse;
    }

    public async Task<Warehouse> Update(Warehouse warehouse, CancellationToken cancellationToken)
    {
        context.Warehouses.Update(warehouse);
        await context.SaveChangesAsync(cancellationToken);
        return warehouse;
    }

    public async Task<Warehouse> Delete(Warehouse warehouse, CancellationToken cancellationToken)
    {
        context.Warehouses.Remove(warehouse);
        await context.SaveChangesAsync(cancellationToken);
        return warehouse;
    }
}