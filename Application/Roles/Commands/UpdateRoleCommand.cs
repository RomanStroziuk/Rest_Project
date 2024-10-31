using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using Domain.Roles;
using MediatR;
using Optional;

namespace Application.Roles.Commands;

public class UpdateRoleCommand : IRequest<Result<Role, RoleExceptions>>
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
}

public class UpdateRoleCommandHandler(IRoleRepository roleRepository) : IRequestHandler<UpdateRoleCommand, Result<Role, RoleExceptions>>
{
    public async Task<Result<Role, RoleExceptions>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.Id);
        
        var role = await roleRepository.GetById(roleId, cancellationToken);

        return await role.Match(
            async r  => 
        {
            var existingRole = await CheckDuplicated(roleId, request.Title, cancellationToken);
            
            return await existingRole.Match(
                r => Task.FromResult<Result<Role, RoleExceptions>>(new RoleAlreadyExistsException(r.Id)),
                async () => await UpdateEntity(r, request.Title, cancellationToken));
        },
            () => Task.FromResult<Result<Role, RoleExceptions>>(new RoleNotFoundException(roleId)));
    }

    private async Task<Result<Role, RoleExceptions>> UpdateEntity(
        Role role,
        string title,
        CancellationToken cancellationToken)
    {
        try
        {
            role.ChangeTitle(title);
            return await roleRepository.Update(role, cancellationToken);
        }
        catch (Exception e)
        {
            return new RoleUnknownException(role.Id, e);
        }
    }

    private async Task<Option<Role>> CheckDuplicated(
        RoleId roleId,
        string name,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.SearchByName(name, cancellationToken);

        return role.Match(
            r => r.Id == roleId ? Option.None<Role>() : Option.Some(r),
            Option.None<Role>);
    }
}