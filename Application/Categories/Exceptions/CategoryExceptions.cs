using Domain.Сategories;


namespace Application.Categories.Exceptions;

public abstract class CategoryException(CategoryId id, string message, Exception? innerException = null)
    : Exception(message, innerException)

{
    public CategoryId Id { get; } = id;
}


public class CategoryNotFoundException(CategoryId id) : CategoryException(id, $"Category under id: {id} not found");

public class CategoryAlreadyExistsException(CategoryId id) : CategoryException(id, $"Category already exists: {id}");

public class CategoryUnknownException(CategoryId id, Exception innerException)
    : CategoryException(id, $"Unknown exception for the categoty under id: {id}", innerException);