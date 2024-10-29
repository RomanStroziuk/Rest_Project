using Application.Warehouses.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class WarehouseErrorHandler
{
    public static ObjectResult ToObjectResult(this WarehouseException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                WarehouseNotFoundException=> StatusCodes.Status404NotFound,
                WarehouseAlreadyExistsException => StatusCodes.Status409Conflict,
                WarehouseUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Warehouse error handler does not implemented")
            }
        };
    }
}