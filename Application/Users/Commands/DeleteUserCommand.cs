using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class DeleteUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid Id { get; set; }
}

public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.Id);
        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match<Task<Result<User, UserException>>>(
            async u => await DeleteEntity(u, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId))
        );

    }

    private async Task<Result<User, UserException>> DeleteEntity(User user, CancellationToken cancellationToken)
    {
        try
        {
            return await userRepository.Delete(user, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(user.Id, exception);
        }
        
    }
}