using Domain.Brands;
using Domain.Sneakers;
using Domain.Сategories;

namespace Application.Sneakers.Exceptions;

public abstract class SneakerException(SneakerId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public SneakerId SneakerId { get; } = id;
}

public class SneakerNotFoundException(SneakerId id) : SneakerException(id, $"Sneaker under id: {id} not found");

public class SneakerAlreadyExistsException(SneakerId id) : SneakerException(id, $"Sneaker already exists: {id}");

public class SneakerBrandNotFoundException(BrandId brandId) : SneakerException(SneakerId.Empty(), $"Brand under id: {brandId} not found");

public class SneakerCategoryNotFoundException(CategoryId categoryId) : SneakerException(SneakerId.Empty(), $"Category under id: {categoryId} not found");

public class SneakerUnknownException(SneakerId id, Exception innerException)
    : SneakerException(id, $"Unknown exception for the sneaker under id: {id}", innerException);