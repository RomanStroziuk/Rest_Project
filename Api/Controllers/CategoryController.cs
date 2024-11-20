using Api.Dtos;
using Api.Dtos.CategoryDtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Categories.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("categories")]
[ApiController]
[Authorize(Roles = "Admin")]


public class CategoryController(ISender sender, ICategoryQueries categoryQueries) : ControllerBase
{
    [HttpGet("list")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await categoryQueries.GetAll(cancellationToken);

        return entities.Select(CategoryDto.FromDomainModel).ToList();
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CategoryDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateCategoryCommand
        {
            
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            c => CategoryDto.FromDomainModel(c),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<CategoryDto>> Update(
        [FromBody] CategoryDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryCommand
        {
            CategoryId = request.Id!.Value,
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            c => CategoryDto.FromDomainModel(c),
            e => e.ToObjectResult());
    }
    [HttpDelete("delete/{categoryId:guid}")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<CategoryDto>> Delete([FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        var input = new DeleteCategoryCommand()
        {
            CategoryId = categoryId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            c => CategoryDto.FromDomainModel(c),
            e => e.ToObjectResult());
    }
    
}