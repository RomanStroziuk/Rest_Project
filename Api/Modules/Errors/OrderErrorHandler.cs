using Microsoft.AspNetCore.Mvc;
using Application.Orders.Exceptions;


namespace Api.Modules.Errors;

public static class OrderErrorHandler
{
    public static ObjectResult ToObjectResult(this OrderException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                OrderNotFoundException or
                    OrderUserNotFoundException or
                    OrderRoleNotFoundException => StatusCodes.Status404NotFound,
                OrderAlreadyExistsException => StatusCodes.Status409Conflict,
                OrderUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Order error handler does not implemented")
            }
        };
    }
}