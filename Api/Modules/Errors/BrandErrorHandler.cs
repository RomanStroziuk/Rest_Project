using Application.Brands.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class BrandErrorHandler
{
    public static ObjectResult ToObjectResult(this BrandException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                BrandNotFoundException => StatusCodes.Status404NotFound,
                BrandAlreadyExistsException => StatusCodes.Status409Conflict,
                BrandUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Brand error handler does not implemented")
            }
        };
    }
}