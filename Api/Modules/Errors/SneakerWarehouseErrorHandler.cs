using Application.SneakerWarehouses.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class SneakerWarehouseErrorHandler
{
    public static ObjectResult ToObjectResult(this SneakerWarehouseException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                SneakerWarehouseNotFoundException or
                    SneakerNotFoundException or
                        WarehouseNotFoundException
                    => StatusCodes.Status404NotFound,
                InsufficientStockException => StatusCodes.Status400BadRequest,
                SneakerWarehouseUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Sneaker warehouse error handler does not implemented")
                
            }
        };
    }
}