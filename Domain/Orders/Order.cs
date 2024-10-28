using Domain.OrderItems;
using Domain.Statuses;
using Domain.Users;

namespace Domain.Orders;

public class Order
{
    public OrderId Id { get; }
    public DateTime OrderDate { get; private set; }
    public int TotalPrice { get; private set; }
    public User? User { get; }
    public UserId UserId { get; }
    public Status? Status { get; }
    public StatusId StatusId { get; }
    public List<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    public Order(OrderId id, UserId userId, StatusId statusId, int totalPrice)
    {
        Id = id;    
        UserId = userId;
        StatusId = statusId;
        OrderDate = DateTime.UtcNow;
        TotalPrice = totalPrice;
    }

    public static Order New(OrderId id, UserId userId, StatusId statusId, int totalPrice)
    => new(id, userId, statusId, totalPrice);
    
    public void AddItem(OrderItem item)
    {
        OrderItems.Add(item);
        //TotalPrice += item.Quantity * item.SneakerWarehouse.Sneaker.Price;
    }
}