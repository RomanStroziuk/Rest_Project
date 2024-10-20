using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Brands.Exceptions;
using Domain.Brands;
using MediatR;
using Optional;

namespace Application.Brands.Commands;

public record UpdateBrandCommand : IRequest<Result<Brand, BrandException>>
{
    public required Guid BrandId { get; init; }
    public required string Name { get; init; }
}

public class UpdateBrandCommandHandler(
    IBrandRepository brandRepository) : IRequestHandler<UpdateBrandCommand, Result<Brand, BrandException>>
{
    public async Task<Result<Brand, BrandException>> Handle(
        UpdateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var brandId = new BrandId(request.BrandId);
        var brand = await brandRepository.GetById(brandId, cancellationToken);

        return await brand.Match(
            async b =>
            {
                var existingBrand = await CheckDuplicated(brandId, request.Name, cancellationToken);

                return await existingBrand.Match(
                    ef => Task.FromResult<Result<Brand, BrandException>>(new BrandAlreadyExistsException(ef.Id)),
                    async () => await UpdateEntity(b, request.Name, cancellationToken));
            },
            () => Task.FromResult<Result<Brand, BrandException>>(new BrandNotFoundException(brandId)));
    }

    private async Task<Result<Brand, BrandException>> UpdateEntity(
        Brand brand,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            brand.UpdateDetails(name);

            return await brandRepository.Update(brand, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BrandUnknownException(brand.Id, exception);
        }
    }

    private async Task<Option<Brand>> CheckDuplicated(
        BrandId brandId,
        string name,
        CancellationToken cancellationToken)
    {
        var brand = await brandRepository.SearchByName(name, cancellationToken);

        return brand.Match(
            b => b.Id == brandId ? Option.None<Brand>() : Option.Some(b),
            Option.None<Brand>);
    }
}