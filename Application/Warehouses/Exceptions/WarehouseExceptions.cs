using Domain.Warehouses;

namespace Application.Warehouses.Exceptions;

public abstract class WarehouseException(WarehouseId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public WarehouseId WarehouseId { get; } = id;
}

public class WarehouseNotFoundException(WarehouseId id) : WarehouseException(id, $"Warehouse with ID {id} not found");

public class WarehouseAlreadyExistsException(WarehouseId id) : WarehouseException(id, $"Warehouse with ID {id} already exists");


public class WarehouseUnknownException(WarehouseId id, Exception innerException)
    : WarehouseException(id, $"Unknown exception for warehouse under ID: {id}", innerException);