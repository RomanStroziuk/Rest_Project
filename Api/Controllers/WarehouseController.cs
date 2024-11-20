using Api.Dtos;
using Api.Dtos.WarehouseDtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Warehouses.Commands;
using Domain.Warehouses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("warehouse")]
[ApiController]
public class WarehouseController(ISender sender, IWarehouseRepository warehouseRepository, IWarehouseQueries warehouseQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var warehouse = await warehouseQueries.GetAll(cancellationToken);
        return warehouse.Select(WarehouseDto.FromDomainModel).ToList();
    }

    [HttpGet("{warehouseId:guid}")]
    public async Task<ActionResult<WarehouseDto>> Get([FromRoute] Guid warehouseId, CancellationToken cancellationToken)
    {
        var entity = await warehouseRepository.GetById(new WarehouseId(warehouseId), cancellationToken);
        
        return entity.Match<ActionResult<WarehouseDto>>(
            w => WarehouseDto.FromDomainModel(w),
            () => NotFound());
    }

    [HttpPost]
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
    
    [HttpPut]
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
    
    [HttpDelete("{warehouseId:guid}")]
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