using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using Domain.Sneakers;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Files.Commands;

public class DeleteSneakerImageCommand : IRequest<Result<Unit, Exception>>
{
    public required Guid Id { get; init; }
}

public class DeleteSneakerImageCommandHandler : IRequestHandler<DeleteSneakerImageCommand, Result<Unit, Exception>>
{
    private readonly ISneakerImageRepository _sneakerImageRepository;

    public DeleteSneakerImageCommandHandler(ISneakerImageRepository sneakerImageRepository)
    {
        _sneakerImageRepository = sneakerImageRepository;
    }

    public async Task<Result<Unit, Exception>> Handle(DeleteSneakerImageCommand request, CancellationToken cancellationToken)
    {
        var sneakerImageId = new SneakerImageId(request.Id);
        var existingImage = await _sneakerImageRepository.GetByIdAsync(sneakerImageId, cancellationToken);

        // Return an exception if the image was not found
        if (existingImage == null)
        {
            return new SneakerImageNotFoundException(sneakerImageId);
        }

        // Attempt to delete the image
        var deleteResult = await _sneakerImageRepository.DeleteAsync(sneakerImageId, cancellationToken);

        // Use implicit conversion for success or failure
        return deleteResult ? Unit.Value : new Exception("Failed to delete sneaker image.");
    }
}