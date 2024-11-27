using Api.Dtos;
using Api.Dtos.UserDtos;

using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("user")]
[ApiController]

public class UserController(ISender sender, IUserRepository userRepository, IUserQueries userQueries) : ControllerBase
{
    [HttpGet("list")]

    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await userQueries.GetAll(cancellationToken);
        return users.Select(UserDto.FromDomainModel).ToList();
    }

    [HttpGet("get/{userId:guid}")]

    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userRepository.GetById(new UserId(userId), cancellationToken);
        
        return entity.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            () => NotFound());
    }

    [HttpPost("register")]

    public async Task<ActionResult<UserDto>> Create([FromBody] UserDto request, CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            RoleId = request.RoleId
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    
    [HttpPost("authenticate")]
    public async Task<ActionResult<string>> LoginUser([FromBody] LoginUserDto loginUserDto,
        CancellationToken cancellationToken)
    {
        var input = new LoginUserCommand
        {
            Email = loginUserDto.email,
            Password = loginUserDto.password
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<string>>
        (token => token,
            e => e.ToObjectResult());
    }
    
    
    
    [HttpPut("update")]

    public async Task<ActionResult<UserDto>> Update([FromBody] UserDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateUserCommand
        {
            Id = request.Id!.Value,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    
    [HttpPut("updateUserInitials/{userId:guid}")]
    public async Task<ActionResult<UserDto>> UpdateFirstAndLastName(
        [FromRoute] Guid userId, 
        [FromBody] UserUpdateInitialsDto request,
        CancellationToken cancellationToken)
    {

        var input = new UpdateUserFirstAndLastNameCommand
        {
            UserId = userId,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut("updatePassword/{userId:guid}")]

    public async Task<ActionResult<UserDto>> UpdatePassword(
        [FromRoute] Guid userId,
        [FromBody] string password,
        CancellationToken cancellationToken)
    {
        var input = new UpdateUserPasswordCommand
        {
            UserId = userId,
            Password = password
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    
    [HttpPut("updateEmail/{userId:guid}")]

    public async Task<ActionResult<UserDto>> UpdateEmail(
        [FromRoute] Guid userId,
        [FromBody] string email,
        CancellationToken cancellationToken)
    {
        var input = new UpdateUserEmailCommand
        {
            UserId = userId,
            Email = email
        };
        
        

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    
    
    [HttpDelete("delete/{userId:guid}")]

    public async Task<ActionResult<UserDto>> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var input = new DeleteUserCommand
        {
            Id = userId
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}