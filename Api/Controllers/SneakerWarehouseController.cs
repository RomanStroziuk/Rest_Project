using Api.Dtos;
using Api.Dtos.SneakerWarehouseDtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.SneakerWarehouses.Commands;
using Domain.SneakerWarehouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("sneaker-warehouse")]
[ApiController] 
[Authorize(Roles = "Admin")]
public class SneakerWarehouseController(ISender sender,
    ISneakerWarehouseRepository sneakerWarehouseRepository,
    ISneakerWarehouseQueries sneakerWarehouseQueries) : ControllerBase
{
    [HttpGet("list")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IReadOnlyList<SneakerWarehouseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var sneakerWarehouses = await sneakerWarehouseQueries.GetAll(cancellationToken);
        return sneakerWarehouses.Select(SneakerWarehouseDto.FromDomainModel).ToList();
    }

    [HttpGet("get/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SneakerWarehouseDto>> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var entity = await sneakerWarehouseRepository.GetById(new SneakerWarehouseId(id), cancellationToken);

        return entity.Match<ActionResult<SneakerWarehouseDto>>(
            s => SneakerWarehouseDto.FromDomainModel(s),
            () => NotFound()
        );
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SneakerWarehouseDto>> Create([FromBody] SneakerWarehouseDto request,
        CancellationToken cancellationToken)
    {
        var input = new AddSneakerToWarehouseCommand()
        {
            SneakerId = request.SneakerId,
            WarehouseId = request.WarehouseId,
            Quantity = request.SneakerQuantity
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<SneakerWarehouseDto>>(
            s => SneakerWarehouseDto.FromDomainModel(s),
            e => e.ToObjectResult());
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SneakerWarehouseDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteSneakerFromWarehouseCommand()
        {
            Id = id
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<SneakerWarehouseDto>>(
            s => SneakerWarehouseDto.FromDomainModel(s),
            e => e.ToObjectResult());
    }
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SneakerWarehouseDto>> Update([FromBody] SneakerWarehouseDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateSneakerQuantityInWarehouseCommand()
        {
            Id = request.Id!.Value,
            Quantity = request.SneakerQuantity
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<SneakerWarehouseDto>>(
            s => SneakerWarehouseDto.FromDomainModel(s),
            e => e.ToObjectResult());
    }
}