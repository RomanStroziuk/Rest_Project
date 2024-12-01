using Domain.OrderItems;
using Domain.Orders;
using Domain.SneakerWarehouses;

namespace Tests.Data;

public class OrderItemData
{
    public static OrderItem MainOrderItem(OrderId orderId, SneakerWarehouseId sneakerWarehouseId)
        => OrderItem.New(OrderItemId.New(), sneakerWarehouseId, orderId, 10, 1000);
}