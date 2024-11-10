using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Statuses;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class StatusRepository(ApplicationDbContext context) : IStatusRepository, IStatusQueries
{
    public async Task<Option<Status>> GetById(StatusId statusId, CancellationToken cancellationToken)
    {
        var entity = await context.Statuses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == statusId, cancellationToken);

        return entity == null ? Option.None<Status>() : Option.Some(entity);
    }
    
    public async Task<Option<Status>> GetByTitle(string title, CancellationToken cancellationToken)
    {
        var entity = await context.Statuses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Title == title, cancellationToken);

        return entity == null ? Option.None<Status>() : Option.Some(entity);
    }
    
    public async Task<IReadOnlyList<Status>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Statuses
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Status>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Statuses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Title == name, cancellationToken);

        return entity == null ? Option.None<Status>() : Option.Some(entity);
    }

    public async Task<Status> Create(Status status, CancellationToken cancellationToken)
    {
        await context.Statuses.AddAsync(status, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return status;
    }

    public async Task<Status> Update(Status status, CancellationToken cancellationToken)
    {
        context.Statuses.Update(status);
        await context.SaveChangesAsync(cancellationToken);
        return status;
    }
    public async Task<Status> Delete(Status status, CancellationToken cancellationToken)
    {
        context.Statuses.Remove(status);
        await context.SaveChangesAsync(cancellationToken);
        return status;
    }
}