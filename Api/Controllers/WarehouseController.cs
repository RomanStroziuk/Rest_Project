using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Warehouses.Commands;
using Domain.Warehouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("warehouse")]
[ApiController]
[Authorize(Roles = "Admin")]

public class WarehouseController(ISender sender, IWarehouseRepository warehouseRepository, IWarehouseQueries warehouseQueries) : ControllerBase
{
    [HttpGet("list")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var warehouse = await warehouseQueries.GetAll(cancellationToken);
        return warehouse.Select(WarehouseDto.FromDomainModel).ToList();
    }

    [HttpGet("get/{warehouseId:guid}")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<WarehouseDto>> Get([FromRoute] Guid warehouseId, CancellationToken cancellationToken)
    {
        var entity = await warehouseRepository.GetById(new WarehouseId(warehouseId), cancellationToken);
        
        return entity.Match<ActionResult<WarehouseDto>>(
            w => WarehouseDto.FromDomainModel(w),
            () => NotFound());
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<WarehouseDto>> Create([FromBody] WarehouseDto request, CancellationToken cancellationToken)
    {
        var input = new CreateWarehouseCommand()
        {
            Location = request.Location,
            TotalQuantity = request.TotalQuantity
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<WarehouseDto>>(
            w => WarehouseDto.FromDomainModel(w),
            e => e.ToObjectResult());
    }
    
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<WarehouseDto>> Update([FromBody] WarehouseDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateWarehouseCommand()
        {
            Id = request.Id!.Value,
            Location = request.Location,
            TotalQuantity = request.TotalQuantity
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<WarehouseDto>>(
            w => WarehouseDto.FromDomainModel(w),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{warehouseId:guid}")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<WarehouseDto>> Delete([FromRoute] Guid warehouseId, CancellationToken cancellationToken)
    {
        var input = new DeleteWarehouseCommand()
        {
            Id = warehouseId
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<WarehouseDto>>(
            w => WarehouseDto.FromDomainModel(w),
            e => e.ToObjectResult());
    }
}