using Application.Statuses.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class StatusErrorHandler
{
    public static ObjectResult ToObjectResult(this StatusExceptions exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                StatusNotFoundException => StatusCodes.Status404NotFound,
                StatusAlreadyExistsException => StatusCodes.Status409Conflict,
                StatusUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Status error handler does not implemented")
            }
        };
    }
}