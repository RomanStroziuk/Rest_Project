using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Sneakers;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class SneakerRepository(ApplicationDbContext context) : ISneakerRepository, ISneakerQueries
{
    public async Task<IReadOnlyList<Sneaker>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Sneakers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Option<Sneaker>> SearchByName(string model, CancellationToken cancellationToken)
    {
        var entity = await context.Sneakers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Model == model, cancellationToken);

        return entity == null ? Option.None<Sneaker>() : Option.Some(entity);
    }
    public async Task<Option<Sneaker>> GetById(SneakerId id, CancellationToken cancellationToken)
    {
        var entity = await context.Sneakers
            .Include( b => b.Brand)
            .Include(c => c.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Sneaker>() : Option.Some(entity);
    }
    
    public async Task<Sneaker> Add(Sneaker sneaker, CancellationToken cancellationToken)
    {
        await context.Sneakers.AddAsync(sneaker, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return sneaker;
    }
    
    public async Task<Sneaker> Update(Sneaker sneaker, CancellationToken cancellationToken)
    {
        context.Sneakers.Update(sneaker);

        await context.SaveChangesAsync(cancellationToken);

        return sneaker;
    }
    
    public async Task<Sneaker> Delete(Sneaker sneaker, CancellationToken cancellationToken)
    {
        context.Sneakers.Remove(sneaker);

        await context.SaveChangesAsync(cancellationToken);

        return sneaker;
    }
}
    
    


    
