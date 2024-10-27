using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.OrderItems;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class OrderItemRepository(ApplicationDbContext context) : IOrderItemRepository, IOrderItemQueries
{
    public async Task<Option<OrderItem>> GetById(OrderItemId orderItemIdId, CancellationToken cancellationToken)
    {
        var entity = await context.OrderItems
            .Include(x => x.Order)
            .Include(x => x.SneakerWarehouse)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == orderItemIdId, cancellationToken);
        
        return entity == null ? Option.None<OrderItem>() : Option.Some(entity);
    }
    
    public async Task<IReadOnlyList<OrderItem>> GetAll(CancellationToken cancellationToken)
    {
        return await context.OrderItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<OrderItem> Create(OrderItem orderItem, CancellationToken cancellationToken)
    {
        await context.OrderItems.AddAsync(orderItem, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return orderItem;
    }

    public async Task<OrderItem> Update(OrderItem orderItem, CancellationToken cancellationToken)
    {
        context.OrderItems.Update(orderItem);
        await context.SaveChangesAsync(cancellationToken);
        return orderItem;
    }

    public async Task<OrderItem> Delete(OrderItem orderItem, CancellationToken cancellationToken)
    {
        context.OrderItems.Remove(orderItem);
        await context.SaveChangesAsync(cancellationToken);
        return orderItem;
    }
}