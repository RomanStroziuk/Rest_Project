using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Brands.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("brands")]
[ApiController]
[Authorize(Roles = "Admin")]

public class BrandsController(ISender sender, IBrandQueries brandQueries) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]

    public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await brandQueries.GetAll(cancellationToken);

        return entities.Select(BrandDto.FromDomainModel).ToList();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<BrandDto>> Create([FromBody] BrandDto request, CancellationToken cancellationToken)
    {
        var input = new CreateBrandCommand
        {
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BrandDto>>(
            b => BrandDto.FromDomainModel(b),
            e => e.ToObjectResult());
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<BrandDto>> Update([FromBody] BrandDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateBrandCommand
        {
            BrandId = request.Id!.Value,
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BrandDto>>(
            b => BrandDto.FromDomainModel(b),
            e => e.ToObjectResult());
    }

    [HttpDelete("{brandId:guid}")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<BrandDto>> Delete([FromRoute] Guid brandId, CancellationToken cancellationToken)
    {
        var input = new DeleteBrandCommand
        {
            BrandId = brandId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BrandDto>>(
            b => BrandDto.FromDomainModel(b),
            e => e.ToObjectResult());
    }
}