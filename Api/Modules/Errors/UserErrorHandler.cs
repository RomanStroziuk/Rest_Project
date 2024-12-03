using Microsoft.AspNetCore.Mvc;
using Application.Users.Exceptions;


namespace Api.Modules.Errors;

public static class UserErrorHandler
{
    public static ObjectResult ToObjectResult(this UserException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                UserNotFoundException or
                    UserRoleNotFoundException or
                    UserPasswordOrEmailNotFoundExceptions or
                    RoleNotFound
                     => StatusCodes.Status404NotFound,
                UserAlreadyExistsException => StatusCodes.Status409Conflict,
                UserUnknownException => StatusCodes.Status500InternalServerError,
                
                _ => throw new NotImplementedException("User error handler does not implemented")
            }
        };
    }
}