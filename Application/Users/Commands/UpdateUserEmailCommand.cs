using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class UpdateUserEmailCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}

public class UpdateUserEmailCommandHandler(IUserRepository repository)
    : IRequestHandler<UpdateUserEmailCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserEmailCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var exsitingUser = await repository.GetById(userId, cancellationToken);

        return await exsitingUser.Match(
            async u => await UpdateEntity(u, request.Email, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User user,
        string email,
        CancellationToken cancellationToken)
    {
        try
        {
            user.UpdateEmail(email);

            return await repository.Update(user, cancellationToken);
        }
        catch (Exception e)
        {
            return new UserUnknownException(user.Id, e);
        }
    }
}