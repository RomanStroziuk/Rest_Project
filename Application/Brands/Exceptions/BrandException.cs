using Domain.Brands;


namespace Application.Brands.Exceptions;

public abstract class BrandException(BrandId id, string message, Exception? innerException = null)
      : Exception(message, innerException)

{

      public BrandId Id { get; } = id;

}


public class BrandNotFoundException(BrandId id) : BrandException(id, $"Brand under id: {id} not found");

public class BrandAlreadyExistsException(BrandId id) : BrandException(id, $"Brand already exists: {id}");

public class BrandUnknownException(BrandId id, Exception innerException)
      : BrandException(id, $"Unknown exception for the brand under id: {id}", innerException);