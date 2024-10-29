using Domain.Users;
using Domain.Orders;
using Domain.Statuses;

namespace Application.Orders.Exceptions;

public abstract class OrderException(OrderId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public OrderId OrderId { get; } = id;
}

public class OrderNotFoundException(OrderId id) : OrderException(id, $"Order under id: {id} not found");

public class OrderAlreadyExistsException(OrderId id) : OrderException(id, $"Order already exists: {id}");

public class OrderUserNotFoundException(UserId userid) : OrderException(OrderId.Empty(), $"User under id: {userid} not found");

public class OrderRoleNotFoundException(StatusId statusId) : OrderException(OrderId.Empty(), $"Role under id: {statusId} not found");



public class OrderUnknownException(OrderId id, Exception innerException)
    : OrderException(id, $"Unknown exception for the order under id: {id}", innerException);