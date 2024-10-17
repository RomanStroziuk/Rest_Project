// //using Application.Sneakers.Exceptions;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Api.Modules.Errors;
//
// public static class SneakerErrorHandler
// {
//     public static ObjectResult ToObjectResult(this SneakerException exception)
//     {
//         return new ObjectResult(exception.Message)
//         {
//             StatusCode = exception switch
//             {
//                 SneakerNotFoundException or
//                     SneakerBrandNotFoundException or
//                          SneakerCategoryNotFoundException => StatusCodes.Status404NotFound,
//                 SneakerAlreadyExistsException => StatusCodes.Status409Conflict,
//                 SneakerUnknownException => StatusCodes.Status500InternalServerError,
//                 _ => throw new NotImplementedException("Sneaker error handler does not implemented")
//             }
//         };
//     }
// }