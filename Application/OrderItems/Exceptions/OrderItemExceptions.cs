
using Domain.OrderItems;
using Domain.Orders;
using Domain.SneakerWarehouses;

namespace Application.OrderItems.Exceptions;

public abstract class OrderItemException : Exception
{
    public OrderItemId? OrderItemId { get; }
    public OrderId? OrderId { get; }
    public SneakerWarehouseId? SneakerWarehouseId { get; }

    protected OrderItemException(OrderItemId? orderItemId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        OrderItemId = orderItemId;
    }
    protected OrderItemException(OrderId? orderId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        OrderId = orderId;
    }
    protected OrderItemException(SneakerWarehouseId? sneakerWarehouseId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        SneakerWarehouseId = sneakerWarehouseId;
    }
}

public class OrderItemNotFoundException(OrderItemId id) 
    : OrderItemException(id, $"Order item with ID {id} not found");

public class OrderNotFoundException(OrderId orderId) 
    : OrderItemException(orderId, $"Order with ID {orderId} not found");

public class SneakerWarehouseNotFoundException(SneakerWarehouseId sneakerWarehouseId) 
    : OrderItemException(sneakerWarehouseId, $"Sneaker warehouse with ID {sneakerWarehouseId} not found");

public class OrderItemUnknownException(OrderItemId id, Exception innerException)
    : OrderItemException(id, $"Unknown exception for order item under ID: {id}", innerException);

public class NotEnoughSneakerException(SneakerWarehouseId sneakerWarehouseId) 
    : OrderItemException(sneakerWarehouseId, $"Not enough sneakers in warehouse with ID {sneakerWarehouseId}");
