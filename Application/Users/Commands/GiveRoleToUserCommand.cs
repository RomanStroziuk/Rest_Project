using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class GiveRoleToUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required Guid RoleId { get; init; }
}

public class GiveRoleToUserCommandHandler(IUserRepository repository, IRoleRepository roleRepository)
    : IRequestHandler<GiveRoleToUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(GiveRoleToUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var roleId = new RoleId(request.RoleId);
        var exsitingUser = await repository.GetById(userId, cancellationToken);
        var exsitingRole = await roleRepository.GetById(roleId, cancellationToken);

        return await exsitingUser.Match(
            async u =>
            {
                return await exsitingRole.Match(
                    async r => await UpdateRole(u, r.Id, cancellationToken),
                    () => Task.FromResult<Result<User, UserException>>(new RoleNotFound(userId, roleId)));
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateRole(User user, RoleId roleId,
        CancellationToken cancellationToken)
    {
        try
        {
            user.SetNewRole(roleId);

            return await repository.Update(user, cancellationToken);
        }
        catch (Exception e)
        {
            return new UserUnknownException(user.Id, e);
        }
    }
}