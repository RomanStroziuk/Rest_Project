using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Statuses.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("status")]
[ApiController]
public class StatusController(ISender sender, IStatusQueries statusQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StatusDto>>> GetAll(CancellationToken cancellationToken)
    {
        var statuses = await statusQueries.GetAll(cancellationToken);
        return statuses.Select(StatusDto.FromDomainModel).ToList();
    }

    [HttpPost]
    
    public async Task<ActionResult<StatusDto>> Create([FromBody] StatusDto request, CancellationToken cancellationToken)
    {
        var input = new CreateStatusCommand
        {
            Title = request.Title
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<StatusDto>>(
            s => StatusDto.FromDomainModel(s),
            e => e.ToObjectResult()
        );
    }
    [HttpPut]
    public async Task<ActionResult<StatusDto>> Update([FromBody] StatusDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateStatusCommand
        {
            Id = request.Id!.Value,
            Title = request.Title
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<StatusDto>>(
            s => StatusDto.FromDomainModel(s),
            e => e.ToObjectResult()
        );
    }

    [HttpDelete("{statusId:guid}")]
    public async Task<ActionResult<StatusDto>> Delete([FromRoute] Guid statusId, CancellationToken cancellationToken)
    {
        var input = new DeleteStatusCommand
        {
            Id = statusId
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<StatusDto>>(
            s => StatusDto.FromDomainModel(s),
            e => e.ToObjectResult()
        );
        
    }
}