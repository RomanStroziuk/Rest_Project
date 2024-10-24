using Domain.Orders;
using Domain.SneakerWarehouses;

namespace Domain.OrderItems;

public class OrderItem
{
    public OrderItemId Id { get; }
    public SneakerWarehouse SneakerWarehouse { get; }
    public SneakerWarehouseId SneakerWarehouseId { get; }
    public Order Order { get; }
    public OrderId OrderId { get; }
    public int Quantity { get; private set; }

    private OrderItem(OrderItemId id, SneakerWarehouseId sneakerWarehouseId, OrderId orderId, int quantity)
    {
        Id = id;
        SneakerWarehouseId = sneakerWarehouseId;
        OrderId = orderId;
        Quantity = quantity;
    }
    public static OrderItem New(OrderItemId id, SneakerWarehouseId sneakerWarehouseId, OrderId orderId, int quantity)
    => new(id, sneakerWarehouseId, orderId, quantity);
}