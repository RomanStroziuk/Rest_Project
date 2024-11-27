using Domain.Orders;
using Domain.SneakerWarehouses;

namespace Domain.OrderItems;

public class OrderItem
{
    public OrderItemId Id { get; }
    public SneakerWarehouse? SneakerWarehouse { get; }
    public SneakerWarehouseId SneakerWarehouseId { get; }
    public Order? Order { get; }
    public OrderId OrderId { get; }
    public int Quantity { get; private set; }
    public int TotalPrice { get; private set; }

    private OrderItem(OrderItemId id, SneakerWarehouseId sneakerWarehouseId, OrderId orderId, int quantity, int totalPrice)
    {
        Id = id;
        SneakerWarehouseId = sneakerWarehouseId;
        OrderId = orderId;
        Quantity = quantity;
        TotalPrice = totalPrice;
    }
    public static OrderItem New(OrderItemId id, SneakerWarehouseId sneakerWarehouseId, OrderId orderId, int quantity, int totalPrice)
    => new(id, sneakerWarehouseId, orderId, quantity, totalPrice);

    public void CalculateTotalPrice(int price)
    {
        TotalPrice = Quantity * price;
    }

    public void UpdateDetails(int quantity, int totalPrice)
    {
        Quantity = quantity;
        TotalPrice = totalPrice;
    }
}
