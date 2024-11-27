using Api.Dtos;
using Api.Dtos.StatusDtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Statuses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("status")]
[ApiController]
[Authorize(Roles = "Admin")]

public class StatusController(ISender sender, IStatusQueries statusQueries) : ControllerBase
{
    [HttpGet("list")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<IReadOnlyList<StatusDto>>> GetAll(CancellationToken cancellationToken)
    {
        var statuses = await statusQueries.GetAll(cancellationToken);
        return statuses.Select(StatusDto.FromDomainModel).ToList();
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]

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
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]

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

    [HttpDelete("delete/{statusId:guid}")]
    [Authorize(Roles = "Admin")]

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