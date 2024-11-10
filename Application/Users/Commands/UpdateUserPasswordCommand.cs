using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class UpdateUserPasswordCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required string Password { get; init; }
}
public class UpdateUserPasswordCommandHandler(IUserRepository repository) : IRequestHandler<UpdateUserPasswordCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var exsistingUser = await repository.GetById(userId, cancellationToken);

        return await exsistingUser.Match(
            async u => await UpdateEntity(u, request.Password, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User user,
        string Password,
        CancellationToken cancellationToken)
    {
        try
        {
            user.UpdatePassword(Password);
            
            return await repository.Update(user, cancellationToken);
        }
        catch (Exception e)
        {
            return new UserUnknownException(user.Id, e);
        }
    }
}