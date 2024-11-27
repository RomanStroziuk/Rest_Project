using Application.OrderItems.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class OrderItemsErrorHandler
{
    public static ObjectResult ToObjectResult(this OrderItemException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                OrderItemNotFoundException => StatusCodes.Status404NotFound,
                OrderNotFoundException => StatusCodes.Status404NotFound,
                SneakerWarehouseNotFoundException => StatusCodes.Status404NotFound,
                OrderItemUnknownException => StatusCodes.Status500InternalServerError,
                NotEnoughSneakerException => StatusCodes.Status400BadRequest,
                _ => throw new NotImplementedException("Order item error handler does not implemented")
            }
        };
    }
}