using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using Domain.Roles;
using MediatR;

namespace Application.Roles.Commands;

public class DeleteRoleCommand : IRequest<Result<Role, RoleExceptions>>
{
    public required Guid Id { get; init; }
}
public class DeleteRoleCommandHandler(IRoleRepository roleRepository) : IRequestHandler<DeleteRoleCommand, Result<Role, RoleExceptions>>
{
    public async Task<Result<Role, RoleExceptions>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.Id);
        var existingRole = await roleRepository.GetById(roleId, cancellationToken);

        return await existingRole.Match<Task<Result<Role, RoleExceptions>>>(
            async s => await DeleteEntity(s, cancellationToken),
            () => Task.FromResult<Result<Role, RoleExceptions>>(new RoleNotFoundException(roleId)));
    }

    private async Task<Result<Role, RoleExceptions>> DeleteEntity(Role role, CancellationToken cancellationToken)
    {
        try
        {
            return await roleRepository.Delete(role, cancellationToken);
        }
        catch (Exception e)
        {
            return new RoleUnknownException(role.Id, e);
        }
    }
}