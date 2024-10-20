using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Sneakers.Exceptions;
using Domain.Brands;
using Domain.Сategories;
using Domain.Sneakers;
using MediatR;

namespace Application.Sneakers.Commands;

public record CreateSneakerCommand : IRequest<Result<Sneaker, SneakerException>>
{
    public required string Model  {get; init;}
    public required int Size { get; init; }
    public required int Price  { get; init; }
    public required Guid BrandId { get; init; }
    public required Guid CategoryId { get; init; }
}

public class CreateSneakerCommandHandler(
    ISneakerRepository sneakerRepository,
    IBrandRepository brandRepository,
    ICategoryRepository categoryRepository)
      :IRequestHandler<CreateSneakerCommand, Result<Sneaker, SneakerException>>
{
    public async Task<Result<Sneaker, SneakerException>> Handle(CreateSneakerCommand request, CancellationToken cancellationToken)
    {
        var brandId = new BrandId(request.BrandId);
        var brand = await brandRepository.GetById(brandId, cancellationToken);

        var categoryId = new CategoryId(request.CategoryId);
        var category = await categoryRepository.GetById(categoryId, cancellationToken);

        return await brand.Match<Task<Result<Sneaker, SneakerException>>>(
            async b =>
            {
                return await category.Match<Task<Result<Sneaker, SneakerException>>>(
                    async c =>
                    {
                        var existingSneaker = await sneakerRepository.SearchByName(request.Model, cancellationToken);

                        return await existingSneaker.Match(
                            s => Task.FromResult<Result<Sneaker, SneakerException>>(new SneakerAlreadyExistsException(s.Id)),
                            async () => await CreateEntity(request.Model, request.Size, request.Price, b.Id, categoryId, cancellationToken));
                        
                    },
                    () => Task.FromResult<Result<Sneaker, SneakerException>>(new SneakerCategoryNotFoundException(categoryId)));
            },
            () => Task.FromResult<Result<Sneaker, SneakerException>>(new SneakerBrandNotFoundException(brandId)));
    }


    private async Task<Result<Sneaker, SneakerException>> CreateEntity(
        string model,
        int size,
        int price,
        BrandId brandId,
        CategoryId categoryId,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Sneaker.New(SneakerId.New(), model, size, price, brandId, categoryId);

            return await sneakerRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new SneakerUnknownException(SneakerId.Empty(), exception);
        }
    }
}