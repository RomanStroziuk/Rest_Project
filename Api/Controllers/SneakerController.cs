using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Sneakers.Commands;
using Domain.Sneakers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("sneaker")]
[ApiController]

public class SneakerController(ISender sender, ISneakerQueries sneakerQueries) : ControllerBase
{
    [HttpGet("list")]
    
    public async Task<ActionResult<IReadOnlyList<SneakerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await sneakerQueries.GetAll(cancellationToken);

        return entities.Select(SneakerDto.FromDomainModel).ToList();
    }

    [HttpGet("get/{sneakerId:guid}")]
    
    public async Task<ActionResult<SneakerDto>> Get([FromRoute] Guid sneakerId, CancellationToken cancellationToken)
    {
        var entity = await sneakerQueries.GetById(new SneakerId(sneakerId), cancellationToken);

        return entity.Match<ActionResult<SneakerDto>>(
            s => SneakerDto.FromDomainModel(s),
            () => NotFound());
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<SneakerDto>> Create([FromBody] SneakerDto request, CancellationToken cancellationToken)
    {
        var input = new CreateSneakerCommand
        {
            Model = request.model,
            Size = request.size,
            Price = request.price,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId
            
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<SneakerDto>>(
            s => SneakerDto.FromDomainModel(s),
            e => e.ToObjectResult());
    }

    [HttpPut("update/{sneakerId:guid}")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<SneakerDto>> Update([FromBody] SneakerDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateSneakerCommand
        {
            SneakerId = request.Id!.Value,
            Model = request.model,
            Size = request.size,
            Price = request.price
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<SneakerDto>>(
            sneaker => SneakerDto.FromDomainModel(sneaker),
            e => e.ToObjectResult());
    }

    [HttpDelete("delete/{sneakerId:guid}")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<SneakerDto>> Delete([FromRoute] Guid sneakerId, CancellationToken cancellationToken)
    {
        var input = new DeleteSneakerCommand
        {
            SneakerId = sneakerId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<SneakerDto>>(
            s => SneakerDto.FromDomainModel(s),
            e => e.ToObjectResult());
    }
}