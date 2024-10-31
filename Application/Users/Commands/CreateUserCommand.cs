using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required Guid RoleId { get; init; }
}
public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository) 
    : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.RoleId);

        var role = await roleRepository.GetById(roleId, cancellationToken);

        return await role.Match<Task<Result<User, UserException>>>(
            async r =>
        {
            var existingUser = await userRepository.GetByFirstNameAndLastName(
                request.FirstName, 
                request.LastName, 
                cancellationToken);

            return await existingUser.Match(
                u => Task.FromResult<Result<User, UserException>>(new UserAlreadyExistsException(u.Id)),
                async () => await CreateEntity(request.FirstName, request.LastName, request.Email, request.Password, roleId, cancellationToken));
        },
        () => Task.FromResult<Result<User, UserException>>(new UserRoleNotFoundException(roleId)));
    }

    private async Task<Result<User, UserException>> CreateEntity(
        string firstName,
        string lastName,
        string email,
        string password,
        RoleId roleId,
        CancellationToken cancellationToken)
    {
        try
        {
            var enity = User.New(UserId.New(), firstName, lastName, email, password, roleId);
            return await userRepository.Create(enity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(UserId.Empty(), exception);
        }
    }
}