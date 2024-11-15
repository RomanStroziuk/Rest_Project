using Domain.Roles;
using Domain.Users;

namespace Application.Users.Exceptions;

public abstract class UserException(UserId userId, string message, Exception? innerException = null)
    : Exception(message, innerException)

{
    public UserId UserId { get; } = userId;
}


public class UserNotFoundException(UserId userId) : UserException(userId, $"User under id: {userId} not found");

public class UserAlreadyExistsException(UserId userId) : UserException(userId, $"User already exists: {userId}");

public class UserRoleNotFoundException(RoleId roleId) : UserException(UserId.Empty(), $"Role under id: {roleId} not found");

public class UserPasswordOrEmailNotFoundExceptions(UserId userId)
    : UserException(userId, $"Wrong password or email address");


public class UserUnknownException(UserId id, Exception innerException)
    : UserException(id, $"Unknown exception for the user under id: {id}", innerException);