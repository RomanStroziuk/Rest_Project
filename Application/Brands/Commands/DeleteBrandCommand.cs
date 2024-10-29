using Application.Brands.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Brands;
using MediatR;

namespace Application.Brands.Commands;

public record DeleteBrandCommand : IRequest<Result<Brand, BrandException>>
{
    public required Guid BrandId { get; init; }
}
public class DeleteBrandCommandHandler(IBrandRepository brandRepository) 
    : IRequestHandler<DeleteBrandCommand, Result<Brand, BrandException>>
{
    public async Task<Result<Brand, BrandException>> Handle(
        DeleteBrandCommand request,
        CancellationToken cancellationToken)
    {
        var brandId = new BrandId(request.BrandId);
        
        var existingBrand = await brandRepository.GetById(brandId, cancellationToken);

        return await existingBrand.Match<Task<Result<Brand, BrandException>>>(
            async b => await DeleteEntity(b, cancellationToken),
            () => Task.FromResult<Result<Brand, BrandException>>(new BrandNotFoundException(brandId)));
    }

    private async Task<Result<Brand, BrandException>> DeleteEntity(Brand brand, CancellationToken cancellationToken)
    {
        try
        {
            return await brandRepository.Delete(brand, cancellationToken);
        }
        catch (Exception e)
        {
            return new BrandUnknownException(brand.Id, e);
        }
    }
}