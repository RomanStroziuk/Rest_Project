using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Brands.Exceptions;
using Domain.Brands;
using MediatR;


namespace Application.Brands.Commands;

public record CreateBrandCommand : IRequest<Result<Brand, BrandException>>
{
    public required string Name { get; init; }
}

public class CreateBrandCommandHandler(
    IBrandRepository brandRepository) : IRequestHandler<CreateBrandCommand, Result<Brand, BrandException>>
{
    public async Task<Result<Brand, BrandException>> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var existingBrand = await brandRepository.SearchByName(request.Name, cancellationToken);

        return await existingBrand.Match(
            b => Task.FromResult<Result<Brand, BrandException>>(new BrandAlreadyExistsException(b.Id)),
            async () => await CreateEntity(request.Name, cancellationToken));
    }

    private async Task<Result<Brand, BrandException>> CreateEntity(
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Brand.New(BrandId.New(), name);

            return await brandRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BrandUnknownException(BrandId.Empty, exception);
        }
    }
}