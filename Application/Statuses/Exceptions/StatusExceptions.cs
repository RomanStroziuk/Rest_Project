using Domain.Statuses;


namespace Application.Statuses.Exceptions;

public abstract class StatusExceptions(StatusId id, string message, Exception? innerException = null)
    : Exception(message, innerException)

{
    public StatusId StatusId { get; } = id;
}


public class StatusNotFoundException(StatusId id) : StatusExceptions(id, $"Status under id: {id} not found");

public class StatusAlreadyExistsException(StatusId id) : StatusExceptions(id, $"Status already exists: {id}");

public class StatusUnknownException(StatusId id, Exception innerException)
    : StatusExceptions(id, $"Unknown exception for the status under id: {id}", innerException);