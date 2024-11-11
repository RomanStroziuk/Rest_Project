using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Optional;
using Domain.Users;
using Domain.Statuses;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository, IOrderQueries
{
    public async Task<Option<Order>> GetById(OrderId orderId, CancellationToken cancellationToken)
    {
        var entity = await context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        return entity == null ? Option.None<Order>() : Option.Some(entity);
    }
    
    public async Task<Option<Order>> GetByIdWithStatus(OrderId orderId, CancellationToken cancellationToken)
    {
        var entity = await context.Orders
            .Include(x => x.User)
            .Include(x => x.Status)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        return entity == null ? Option.None<Order>() : Option.Some(entity);
    }
    
    
    
    public async Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> Create(Order order, CancellationToken cancellationToken)
    {
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Order> Update(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Order> Delete(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }
    
    public async Task<Option<Order>> SearchByStatusAndUser(UserId userId, StatusId statusId, CancellationToken cancellationToken)
    {
        // Logic to find orders based on the userId and statusId.
        // For example, you can query the database using these two parameters.
        var order = await context.Orders
            .Where(o => o.UserId == userId && o.StatusId == statusId)
            .FirstOrDefaultAsync(cancellationToken);

        return order == null ? Option.None<Order>() : Option.Some(order);
    }

}