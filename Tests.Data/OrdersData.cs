using Domain.Orders;
using Domain.Statuses;
using Domain.Users;

namespace Tests.Data;

public static class OrdersData
{
    public static Order MainOrder(UserId userId, StatusId statusId)
        => Order.New(OrderId.New(), userId, statusId);
}