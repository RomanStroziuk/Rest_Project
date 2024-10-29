using Domain.Roles;


namespace Application.Roles.Exceptions;

public abstract class RoleExceptions(RoleId id, string message, Exception? innerException = null)
    : Exception(message, innerException)

{
    public RoleId RoleId { get; } = id;
}


public class RoleNotFoundException(RoleId id) : RoleExceptions(id, $"Role under id: {id} not found");

public class RoleAlreadyExistsException(RoleId id) : RoleExceptions(id, $"Role already exists: {id}");

public class RoleUnknownException(RoleId id, Exception innerException)
    : RoleExceptions(id, $"Unknown exception for the role under id: {id}", innerException);