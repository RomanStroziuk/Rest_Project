using Domain.Sneakers;
using Domain.SneakerWarehouses;
using Domain.Warehouses;

namespace Application.SneakerWarehouses.Exceptions;

public abstract class SneakerWarehouseException : Exception
{
    public SneakerWarehouseId? SneakerWarehouseId { get; }
    public SneakerId? SneakerId { get; }
    public WarehouseId? WarehouseId { get; }
    
    protected SneakerWarehouseException(SneakerWarehouseId id, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        SneakerWarehouseId = id;
    }

    protected SneakerWarehouseException(SneakerId? sneakerId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        SneakerId = sneakerId;
    }

    protected SneakerWarehouseException(WarehouseId? warehouseId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        WarehouseId = warehouseId;
    }
}
public class SneakerWarehouseNotFoundException(SneakerWarehouseId id) : SneakerWarehouseException(id, $"Record about sneaker warehouse with ID {id} not found");
public class SneakerNotFoundException(SneakerId sneakerId) : SneakerWarehouseException(sneakerId, $"Sneaker with ID {sneakerId} not found");

public class WarehouseNotFoundException(WarehouseId warehouseId) : SneakerWarehouseException(warehouseId, $"Warehouse with ID {warehouseId} not found");

public class InsufficientStockException(WarehouseId id, int availableQuantity, int requestedQuantity)
    : SneakerWarehouseException(id, $"Insufficient stock in warehouse {id}. Available: {availableQuantity}, requested: {requestedQuantity}");

public class SneakerWarehouseUnknownException(SneakerWarehouseId id, Exception innerException)
    : SneakerWarehouseException(id, $"Unknown exception for sneaker warehouse under ID: {id}", innerException);
