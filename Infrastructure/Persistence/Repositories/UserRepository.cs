using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces;

using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository, IUserQueries
{
    public async Task<Option<User>> GetById(UserId userId, CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .Include(x => x.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return entity == null ? Option.None<User>() : Option.Some(entity);
    }
    public async Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Option<User>> GetByEmailAndPassword(string email, string password, CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .Include(x=>x.Role)
            .AsNoTracking()
            .SingleOrDefaultAsync(x=>x.Email == email & x.Password == password, cancellationToken);
        
        return entity == null ? Option.None<User>() : Option.Some(entity);
    }
    public async Task<User> Create(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> Update(User user, CancellationToken cancellationToken)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> Delete(User user, CancellationToken cancellationToken)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<Option<User>> GetByFirstNameAndLastName(string firstName, string lastName, CancellationToken cancellationToken)
    {
        var entity = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FirstName == firstName && x.LastName == lastName, cancellationToken);
        return entity == null ? Option.None<User>() : Option.Some(entity);
    }
}