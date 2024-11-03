using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Commands;
using Domain.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("role")]
[ApiController]
public class RoleController(ISender sender, IRoleRepository roleRepository, IRoleQueries roleQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await roleQueries.GetAll(cancellationToken);
        return roles.Select(RoleDto.FromDomainModel).ToList();
    }

    [HttpGet("{roleId:guid}")]
    public async Task<ActionResult<RoleDto>> Get([FromRoute] Guid roleId, CancellationToken cancellationToken)
    {
        var entity = await roleRepository.GetById(new RoleId(roleId), cancellationToken);
        
        return entity.Match<ActionResult<RoleDto>>(
            r => RoleDto.FromDomainModel(r),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] RoleDto request, CancellationToken cancellationToken)
    {
        var input = new CreateRoleCommand
        {
            Title = request.Title
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<RoleDto>>(
            r => RoleDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }
    
    [HttpPut]
    public async Task<ActionResult<RoleDto>> Update([FromBody] RoleDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateRoleCommand
        {
            Id = request.Id!.Value,
            Title = request.Title
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<RoleDto>>(
            r => RoleDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("{roleId:guid}")]
    public async Task<ActionResult<RoleDto>> Delete([FromRoute] Guid roleId, CancellationToken cancellationToken)
    {
        var input = new DeleteRoleCommand
        {
            Id = roleId
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<RoleDto>>(
            r => RoleDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }
}