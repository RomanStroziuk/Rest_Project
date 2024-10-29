using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository, IOrderQueries
{
    public async Task<Option<Order>> GetById(OrderId orderId, CancellationToken cancellationToken)
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

}